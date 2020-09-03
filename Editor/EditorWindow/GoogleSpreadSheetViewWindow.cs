#if ODIN_INSPECTOR


namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.EditorWindow
{
    using System.Collections.Generic;
    using SheetsImporter;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEngine;

    public class GoogleSpreadSheetViewWindow : OdinEditorWindow
    {
        #region static data
        
        public static GoogleSpreadSheetViewWindow Open(IEnumerable<SheetData> sheets)
        {
            var window = GetWindow<GoogleSpreadSheetViewWindow>();
            window.Initialize(sheets);
            window.Show();
            return window;
        }
        
        #endregion

#if ODIN_INSPECTOR
        [InlineEditor()]
#endif
        public List<SpreadsheetSheetView> tables = new List<SpreadsheetSheetView>();

        public void Initialize(IEnumerable<SheetData> sheets)
        {

            foreach (var sheetView in tables) {
                if (sheetView) {
                    DestroyImmediate(sheetView); 
                }
            }
            
            tables.Clear();

            foreach (var sheetData in sheets) {
                var view = ScriptableObject.CreateInstance<SpreadsheetSheetView>();
                view.Initialize(sheetData);
                tables.Add(view);
            }
            
        }
        
    }
}

#endif