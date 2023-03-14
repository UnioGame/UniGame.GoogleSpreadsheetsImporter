namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using Editor;

    public class SpreadsheetImporter<T> : BaseSpreadsheetImporter
        where T : SerializableSpreadsheetImporter
    {
        public List<T> importers = new List<T>();

        public override bool CanImport => importers.Any(x => x.CanImport);
        public override bool CanExport => importers.Any(x => x.CanExport);

        public override IEnumerable<object> Load()
        {
            return importers;
        }

        public override ISpreadsheetData ExportObjects(IEnumerable<object> source, ISpreadsheetData data)
        {
            var items = source.OfType<ISpreadsheetAssetsExporter>()
                .Where(x => x.CanExport);
            
            foreach (var importer in items) 
                importer.Export(data);

            return data;
        }

        public sealed override IEnumerable<object> ImportObjects(IEnumerable<object> source,ISpreadsheetData spreadsheetData)
        {
            var result = new List<object>();

            var items = OnPreImport(source.OfType<T>());
            
            foreach (var importer in items.Where(x=>x.CanImport)) {
                result.AddRange(importer.Import(spreadsheetData));
            }
            
            result = OnPostImport(result).ToList();
            return result;
        }

        
        protected override void OnInitialize(IGoogleSpreadsheetClient client)
        {
            base.Initialize(client);

            foreach (var importer in importers)
            {
                importer.Initialize(client);
            }
        }

        
        protected virtual IEnumerable<T> OnPreImport(IEnumerable<T> sourceImporters)
        {
            return sourceImporters;
        }
        
        protected virtual IEnumerable<object> OnPostImport(IEnumerable<object> importedAssets)
        {
            return importedAssets;
        }
    }
}