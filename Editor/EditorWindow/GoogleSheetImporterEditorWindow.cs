#if ODIN_INSPECTOR

namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.EditorWindow
{
    using UniModules.Editor;
    using SheetsImporter;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;

    public class GoogleSheetImporterEditorWindow : OdinEditorWindow
    {
        #region static data
        
        [MenuItem("UniGame/Google/SpreadsheetImporterWindow")]
        public static void Open()
        {
            var window = GetWindow<GoogleSheetImporterEditorWindow>();
            window.Show();
        }
        
        #endregion

        [SerializeField]
        [HideLabel]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden, Expanded = true)]
        public GoogleSpreadsheetImporter _googleSheetImporter;

        
        #region private methods

        public static GoogleSpreadsheetImporter GetGoogleSpreadsheetImporter()
        {
            //load importer asset
            var importer = AssetEditorTools.GetAsset<GoogleSpreadsheetImporter>();
            if (!importer) {
                importer = CreateInstance<GoogleSpreadsheetImporter>();
                importer.SaveAsset(nameof(GoogleSpreadsheetImporter), GoogleSheetImporterEditorConstants.DefaultGoogleSheetImporterPath);
            }

            return importer;
        }
        
        protected override void OnEnable()
        {
            _googleSheetImporter = GetGoogleSpreadsheetImporter();
            base.OnEnable();
        }

        #endregion
        
    }
}
#endif
