using System;
using Sirenix.OdinInspector;
using UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter;

namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.EditorWindow
{
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