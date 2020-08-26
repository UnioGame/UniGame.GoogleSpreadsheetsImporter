namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.Importers
{
    using System.Collections.Generic;
    using System.Linq;
    using Abstract;

    public class SpreadsheetImporter<T> : BaseSpreadsheetImporter
        where T : SerializableSpreadsheetImporter
    {
        public  List<T> importers = new List<T>();
    
        public override IEnumerable<object> Load()
        {
            return importers;
        }

        public override SpreadsheetData ExportObjects(IEnumerable<object> source,SpreadsheetData data)
        {
            foreach (var importer in source.OfType<ISpreadsheetAssetsExporter>()) {
                importer.Export(data);
            }

            return data;
        }

        public sealed override IEnumerable<object> ImportObjects(IEnumerable<object> source,SpreadsheetData spreadsheetData)
        {
            var result = new List<object>();
            
            foreach (var importer in OnPreImport(source.OfType<T>())) {
                result.AddRange(importer.Import(spreadsheetData));
            }
            
            result = OnPostImport(result).ToList();
            return result;
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