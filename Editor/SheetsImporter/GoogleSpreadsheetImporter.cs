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
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/Google/GoogleSpreadSheetImporter", fileName = nameof(GoogleSpreadsheetImporter))]
    public class GoogleSpreadsheetImporter : ScriptableObject, ILifeTimeContext
    {
        private const int DefaultButtonsWidth = 100;

        #region inspector

        /// <summary>
        /// list of assets linked by attributes
        /// </summary>
        [Space(8)]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty()]
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

        #endregion

        #region private data

        private ISpreadsheetStatus _clientStatus;

        private LifeTimeDefinition _lifeTime;

        private GoogleSpreadsheetClient _sheetClient;

        #endregion

        #region public properties

        public bool IsValidToConnect => sheetsIds.Any(x => !string.IsNullOrEmpty(x));

        public bool HasConnectedSheets => _clientStatus.HasConnectedSheets;

        public ILifeTime LifeTime => (_lifeTime = _lifeTime ?? new LifeTimeDefinition());

        #endregion

        #region public methods

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HorizontalGroup("Sources", DefaultButtonsWidth)]
        [Sirenix.OdinInspector.VerticalGroup("Sources/Source Commands", PaddingTop = 30)]
        [Sirenix.OdinInspector.Button("Import All")]
        [Sirenix.OdinInspector.EnableIf("HasConnectedSheets")]
#endif
        public void Import()
        {
            sheetsItemsHandler.Import();
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.VerticalGroup("Sources/Source Commands")]
        [Sirenix.OdinInspector.Button("Export All")]
        [Sirenix.OdinInspector.EnableIf("HasConnectedSheets")]
#endif
        public void Export()
        {
            sheetsItemsHandler.Export();
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HorizontalGroup("Sheets", DefaultButtonsWidth)]
        [Sirenix.OdinInspector.BoxGroup("Sheets/Commands", false)]
        [Sirenix.OdinInspector.Button("Show")]
#endif
        public void ShowSpreadSheets()
        {
            GoogleSpreadSheetViewWindow.Open(_sheetClient.GetSheets());
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ButtonGroup()]
        [Sirenix.OdinInspector.Button("Reload Spreadsheets")]
        [Sirenix.OdinInspector.EnableIf("HasConnectedSheets")]
#endif
        private void ReloadSpreadsheetsData()
        {
            foreach (var sheetsId in sheetsIds) {
                if (string.IsNullOrEmpty(sheetsId))
                    continue;
                _sheetClient.Reload(sheetsId);
            }
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

            _sheetClient.Connect();

            LifeTime.AddDispose(_sheetClient);

            ReloadSpreadsheetsData();

            sheetsItemsHandler.Initialize(_sheetClient);
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

        private void LoadTypeConverters()
        {
            typeConverters = typeConverters ? typeConverters : ObjectTypeConverter.TypeConverters;
        }

        private void OnEnable()
        {
            CreateClient();
            LoadTypeConverters();
        }

        private GoogleSpreadsheetClient CreateClient()
        {
            _sheetClient = new GoogleSpreadsheetClient(
                user,
                credentialsPath,
                GoogleSheetImporterConstants.ApplicationName,
                GoogleSpreadsheetConnection.WriteScope);
            _clientStatus = _sheetClient.Status;
            LifeTime.AddDispose(_sheetClient);
            return _sheetClient;
        }

        #endregion
    }
}