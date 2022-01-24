using System.Collections.Generic;
using System.Linq;
using UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.Abstract;

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
        
        [MenuItem("UniGame/Google/Spreadsheet ImporterWindow")]
        public static void Open()
        {
            var window = GetWindow<GoogleSheetImporterEditorWindow>(true, "Google Sheets", true);
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

        private GoogleImportersCommonOperations _operations;
        private List<ISpreadsheetAssetsHandler> _assetsHandlers;
        private OdinMenuTree _menuTree;
        
        #region private methods

        protected override OdinMenuTree BuildMenuTree()
        {
            _googleSheetImporter = GetGoogleSpreadsheetImporter();

            _menuTree = new OdinMenuTree();
            var spreadSheerHandler = _googleSheetImporter.sheetsItemsHandler;
            
            _assetsHandlers = spreadSheerHandler.Importers.ToList();
            
            _operations = new GoogleImportersCommonOperations(_googleSheetImporter);
            
            _menuTree.Add("Commands",_operations);
            
            foreach (var importer in _assetsHandlers)
                _menuTree.Add($"Import & Export/{importer.Name}",importer);
            
            _menuTree.Add("Configuration",_googleSheetImporter);

            if(_googleSheetImporter.autoConnect)
                _operations.Reconnect();

            _menuTree.Selection.SelectionChanged -= OnSelectionChange;
            _menuTree.Selection.SelectionChanged += OnSelectionChange;
            
            return _menuTree;
        }

        private void OnSelectionChange(SelectionChangedType _)
        {
            var selection = _menuTree.Selection.SelectedValue;
            if(selection is ISelectable selectable)
                selectable.Select();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
    
            if(_menuTree!=null)
                _menuTree.Selection.SelectionChanged -= OnSelectionChange;
        }

        #endregion

    }
}
#endif
