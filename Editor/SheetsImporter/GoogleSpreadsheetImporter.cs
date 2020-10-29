using UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.CoProcessors;

namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using EditorWindow;
    using GoogleSpreadsheets.Editor.SheetsImporter;
    using TypeConverters.Editor;
    using UniCore.Runtime.DataFlow;
    using UnityEngine;

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
        [Sirenix.OdinInspector.BoxGroup(nameof(user), false)]
        [Sirenix.OdinInspector.FilePath]
#endif
        public string credentialsPath;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup(nameof(user))]
#endif
        public string user = "user";

        /// <summary>
        /// list of target sheets
        /// </summary>
        [Space(4)]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HorizontalGroup("Sheets")]
        [Sirenix.OdinInspector.BoxGroup("Sheets/Sheets Ids", false)]
        [Sirenix.OdinInspector.InfoBox("Add any valid spreadsheet id's")]
#endif
        public List<string> sheetsIds = new List<string>();


        /// <summary>
        /// list of assets linked by attributes
        /// </summary>
        [Space(8)]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
        [Sirenix.OdinInspector.HorizontalGroup("Sources")]
        [Sirenix.OdinInspector.BoxGroup("Sources/Assets Handlers")]
#endif
        public SpreadsheetImportersHandler sheetsItemsHandler = new SpreadsheetImportersHandler();

        [Space(8)]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor(Sirenix.OdinInspector.InlineEditorObjectFieldModes.Boxed, Expanded = true)]
        [Sirenix.OdinInspector.HideLabel]
        [Sirenix.OdinInspector.BoxGroup("Converters")]
#endif
        public ObjectTypeConverter typeConverters;

        [Space(8)]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor(Sirenix.OdinInspector.InlineEditorObjectFieldModes.Boxed, Expanded = true)]
        [Sirenix.OdinInspector.HideLabel]
        [Sirenix.OdinInspector.BoxGroup("Co-Processors")]
#endif
        public CoProcessor _coProcessors;

        #endregion

        #region private data

        private LifeTimeDefinition _lifeTime;

        private GoogleSpreadsheetClient _sheetClient;

        #endregion

        #region public properties

        public bool IsValidToConnect => sheetsIds.Any(x => !string.IsNullOrEmpty(x));

        public bool HasConnectedSheets => Status.HasConnectedSheets;

        public ILifeTime LifeTime => (_lifeTime = _lifeTime ?? new LifeTimeDefinition());

        public IGoogleSpreadsheetClient Client => (_sheetClient = _sheetClient ?? CreateClient());

        public IGooglsSpreadsheetClientStatus Status => Client.Status;
        
        #endregion

        #region public methods

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HorizontalGroup("Sources", DefaultButtonsWidth)]
        [Sirenix.OdinInspector.VerticalGroup("Sources/Source Commands", PaddingTop = 30)]
        [Sirenix.OdinInspector.Button("Import All")]
        [Sirenix.OdinInspector.EnableIf("HasConnectedSheets")]
#endif
        public void Import() => sheetsItemsHandler.Import();

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.VerticalGroup("Sources/Source Commands")]
        [Sirenix.OdinInspector.Button("Export All")]
        [Sirenix.OdinInspector.EnableIf("HasConnectedSheets")]
#endif
        public void Export() => sheetsItemsHandler.Export();

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HorizontalGroup("Sheets", DefaultButtonsWidth)]
        [Sirenix.OdinInspector.BoxGroup("Sheets/Commands", false)]
        [Sirenix.OdinInspector.Button("Show")]
#endif
        public void ShowSpreadSheets()
        {
#if ODIN_INSPECTOR
            GoogleSpreadSheetViewWindow.Open(Client.GetSheets());
#endif
        }
            
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ButtonGroup()]
        [Sirenix.OdinInspector.Button("Reload Spreadsheets")]
        [Sirenix.OdinInspector.EnableIf("HasConnectedSheets")]
#endif
        public void ReloadSpreadsheetsData()
        {
            ReconnectToSpreadsheets();
            Client.ReloadAll();
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ButtonGroup()]
        [Sirenix.OdinInspector.EnableIf("IsValidToConnect")]
        [Sirenix.OdinInspector.Button("Connect Spreadsheets")]
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
        [Sirenix.OdinInspector.ButtonGroup()]
        [Sirenix.OdinInspector.Button("Reset Credentials")]
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
            foreach (var sheetsId in sheetsIds) {
                if (string.IsNullOrEmpty(sheetsId))
                    continue;
                
                Client.ConnectToSpreadsheet(sheetsId);
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