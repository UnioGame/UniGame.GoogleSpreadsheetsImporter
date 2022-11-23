using UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.CoProcessors;

namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using global::UniGame.Core.Runtime;
    using EditorWindow;
    using GoogleSpreadsheets.Editor.SheetsImporter;
    using TypeConverters.Editor;
    using UniCore.Runtime.DataFlow;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
        
   
    [CreateAssetMenu(menuName = "UniGame/Google/GoogleSpreadSheetImporter", fileName = nameof(GoogleSpreadsheetImporter))]
    public class GoogleSpreadsheetImporter : ScriptableObject, ILifeTimeContext
    {
        private const int DefaultButtonsWidth = 100;

        #region inspector

        /// <summary>
        /// credential profile file
        /// https://developers.google.com/sheets/api/quickstart/dotnet
        /// </summary>
#if ODIN_INSPECTOR
        [BoxGroup(nameof(user), false)] [FilePath]
#endif
        public string credentialsPath;

#if ODIN_INSPECTOR
        [BoxGroup(nameof(user))]
#endif
        public string user = "user";

        [Header("try to auto-connect to spreadsheets when window opened")]
        public bool autoConnect = true;
        
        /// <summary>
        /// list of target sheets
        /// </summary>
        [Space(4)]
#if ODIN_INSPECTOR
        [HorizontalGroup("Sheets")]
        [BoxGroup("Sheets/Sheets Ids", false)]
        [InfoBox("Add any valid spreadsheet id's")]
        [TableList]
#endif
        public List<SpreadSheetInfo> sheets = new List<SpreadSheetInfo>();


        /// <summary>
        /// list of assets linked by attributes
        /// </summary>
        [Space(8)]
#if ODIN_INSPECTOR
        [InlineProperty]
        [HideLabel]
        [HorizontalGroup("Sources")]
        [BoxGroup("Sources/Assets Handlers")]
#endif
        public SpreadsheetImportersHandler sheetsItemsHandler = new SpreadsheetImportersHandler();

        [Space(8)]
#if ODIN_INSPECTOR
        [InlineEditor(InlineEditorObjectFieldModes.Boxed, Expanded = true)]
        [HideLabel]
        [BoxGroup("Converters")]
#endif
        public ObjectTypeConverter typeConverters;

        [Space(8)]
#if ODIN_INSPECTOR
        [InlineEditor(InlineEditorObjectFieldModes.Boxed, Expanded = true)]
        [HideLabel]
        [BoxGroup("Co-Processors")]
#endif
        public CoProcessor _coProcessors;

        #endregion

        #region private data

        private LifeTimeDefinition _lifeTime;

        private GoogleSpreadsheetClient _sheetClient;

        #endregion

        #region public properties

        public bool IsValidToConnect => sheets.Any(x => !string.IsNullOrEmpty(x.id));

        public bool HasConnectedSheets => Client.IsConnected;

        public ILifeTime LifeTime => (_lifeTime ??= new LifeTimeDefinition());

        public IGoogleSpreadsheetClient Client => (_sheetClient ??= CreateClient());

        public IGooglsSpreadsheetClientStatus Status => Client.Status;

        #endregion

        #region public methods

#if ODIN_INSPECTOR
        [HorizontalGroup("Sources", DefaultButtonsWidth)]
        [VerticalGroup("Sources/Source Commands", PaddingTop = 30)]
        [Button("Import All")]
        [EnableIf(nameof(HasConnectedSheets))]
#endif
        public void Import() => sheetsItemsHandler.Import();

#if ODIN_INSPECTOR
        [VerticalGroup("Sources/Source Commands")]
        [Button("Export All")]
        [EnableIf(nameof(HasConnectedSheets))]
#endif
        public void Export() => sheetsItemsHandler.Export();

#if ODIN_INSPECTOR
        [HorizontalGroup("Sheets", DefaultButtonsWidth)]
        [BoxGroup("Sheets/Commands", false)]
        [Button("Show")]
#endif
        public void ShowSpreadSheets()
        {
#if ODIN_INSPECTOR
            GoogleSpreadSheetViewWindow.Open(Client.Sheets);
#endif
        }

#if ODIN_INSPECTOR
        //[ButtonGroup()]
        //[Button("Reload Spreadsheets")]
        //[EnableIf(nameof(HasConnectedSheets))]
#endif
        public void ReloadSpreadsheetsData()
        {
            ReconnectToSpreadsheets(); 
            //Client.ReloadAll();
        }

#if ODIN_INSPECTOR
        [ButtonGroup()]
        [EnableIf(nameof(IsValidToConnect))]
        [Button("Connect Spreadsheets")]
#endif
        public void Reconnect()
        {
            _lifeTime?.Release();

            LoadTypeConverters();

            Client.Connect(user, credentialsPath);

            LifeTime.AddDispose(Client);

            ReloadSpreadsheetsData();

            sheetsItemsHandler.Initialize(Client);
        }

#if ODIN_INSPECTOR
        [ButtonGroup()]
        [Button("Reset Credentials")]
#endif
        public void ResetCredentials()
        {
            if (Directory.Exists(GoogleSheetImporterConstants.TokenKey))
                Directory.Delete(GoogleSheetImporterConstants.TokenKey, true);

            _lifeTime?.Release();
        }

        #endregion

        #region private methods

        private void ReconnectToSpreadsheets()
        {
            foreach (var sheet in sheets)
            {
                if (string.IsNullOrEmpty(sheet.id))
                    continue;
                Client.ConnectToSpreadsheet(sheet.id);
            }
        }

        private void LoadTypeConverters()
        {
            typeConverters = typeConverters ? typeConverters : ObjectTypeConverter.TypeConverters;
        }

        private void LoadNestedProcessors()
        {
            _coProcessors = _coProcessors ? _coProcessors : CoProcessor.Processor;
        }

        private void OnEnable()
        {
            LoadTypeConverters();
            LoadNestedProcessors();
        }

        private GoogleSpreadsheetClient CreateClient()
        {
            var client = new GoogleSpreadsheetClient(
                user,
                credentialsPath,
                GoogleSheetImporterConstants.ApplicationName,
                GoogleSpreadsheetConnection.WriteScope);

            LifeTime.AddDispose(client);
            return client;
        }

        #endregion
    }
}