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
}
#endif
