namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System.Collections.Generic;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    public class SheetImporterAsset<TImporter> : BaseSpreadsheetProcessor
        where TImporter : SerializableSpreadsheetProcessor
    {
#if ODIN_INSPECTOR
        [InlineProperty]
        [HideLabel]
#endif
        public TImporter importer;

        public override bool CanImport => importer.CanImport;
        public override bool CanExport => importer.CanExport;
        public override string Name => importer.Name;
        
        public override IEnumerable<object> Load()
        {
            return importer.Load();
        }

        public override IEnumerable<object> ImportObjects(IEnumerable<object> source, ISpreadsheetData spreadsheetData)
        {
            return importer.ImportObjects(source, spreadsheetData);
        }

        public override ISpreadsheetData ExportObjects(IEnumerable<object> source, ISpreadsheetData spreadsheetData)
        {
            return importer.ExportObjects(source, spreadsheetData);
        }

        public override string FormatName(string assetName) => importer.Name;
        
        protected override void OnReset()
        {
            importer.Reset();
        }
    }
}