using System;

#if ODIN_INSPECTOR

namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.EditorWindow
{
    using UniModules.Editor;
    using SheetsImporter;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;

    public class GoogleSheetImporterEditorWindow : OdinMenuEditorWindow
    {
        #region static data
        
        [MenuItem("UniGame/Google/SpreadsheetImporterWindow")]
        public static void Open()
        {
            var window = GetWindow<GoogleSheetImporterEditorWindow>();
            window.Show();
        }
        
        public static GoogleSpreadsheetImporter GetGoogleSpreadsheetImporter()
        {
            //load importer asset
            var importer = AssetEditorTools.GetAsset<GoogleSpreadsheetImporter>();
            if (importer) return importer;
            
            importer = CreateInstance<GoogleSpreadsheetImporter>();
            importer.SaveAsset(nameof(GoogleSpreadsheetImporter), GoogleSheetImporterEditorConstants.DefaultGoogleSheetImporterPath);

            return importer;
        }

        #endregion

        [SerializeField]
        [HideLabel]
        [HideInInspector]
        public GoogleSpreadsheetImporter _googleSheetImporter;

        
        #region private methods

        protected override OdinMenuTree BuildMenuTree()
        {
            _googleSheetImporter = GetGoogleSpreadsheetImporter();

            var tree = new OdinMenuTree();
            var spreadSheerHandler = _googleSheetImporter.sheetsItemsHandler;
            var importers = spreadSheerHandler.Importers;
            
            tree.Add("Commands",new GoogleImportersCommonOperations(_googleSheetImporter));
            
            foreach (var importer in importers)
            {
                tree.Add($"Import & Export/{importer.Name}",importer);
            }
            
            tree.Add("Configuration",_googleSheetImporter);

            return tree;
        }
        
        
        #endregion

    }

    [Serializable]
    public class GoogleImportersCommonOperations
    {
        private readonly GoogleSpreadsheetImporter _importer;

#if ODIN_INSPECTOR
        [ButtonGroup("Importers")]
        [Button("Import All")]
        [EnableIf(nameof(HasConnectedSheets))]
#endif
        public void Import() => _importer.Import();

#if ODIN_INSPECTOR
        [ButtonGroup("Importers")]
        [Button("Export All")]
        [EnableIf(nameof(HasConnectedSheets))]
#endif
        public void Export() => _importer.Export();
        
#if ODIN_INSPECTOR
        [ButtonGroup()]
        [EnableIf(nameof(IsValidToConnect))]
        [Button("Connect Spreadsheets")]
#endif
        public void Reconnect() => _importer.Reconnect();
        
        public GoogleImportersCommonOperations(GoogleSpreadsheetImporter importer)
        {
            _importer = importer;
        }
        
        
        private bool HasConnectedSheets() =>  _importer && _importer.HasConnectedSheets;

        private bool IsValidToConnect() => _importer && _importer.IsValidToConnect;
        
    }
    
}
#endif
