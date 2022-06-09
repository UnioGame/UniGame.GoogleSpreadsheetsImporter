namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.Importers
{
    using System.Collections.Generic;
    using System.Linq;
    using Abstract;
    using UniRx;

    public class SpreadsheetImporter<T> : BaseSpreadsheetImporter
        where T : SerializableSpreadsheetImporter
    {
        public List<T> importers = new List<T>();

        public override bool CanImport => importers.Any(x => x.CanImport);
        public override bool CanExport => importers.Any(x => x.CanExport);

        public override void Initialize(IGoogleSpreadsheetClient client)
        {
            base.Initialize(client);

            foreach (var importer in importers)
            {
                importer.Initialize(client);
                importer.ExportCommand.
                    Where(x=>x.CanExport).
                    Do(x => ExportObjects(new[] {x}, client.SpreadsheetData)).
                    Subscribe().
                    AddTo(LifeTime);

                importer.ImportCommand.
                    Where(x=>x.CanImport).
                    Do(x => ImportObjects(new []{x}, client.SpreadsheetData)).
                    Subscribe().
                    AddTo(LifeTime);
            }
        }

        public override IEnumerable<object> Load()
        {
            return importers;
        }

        public override ISpreadsheetData ExportObjects(IEnumerable<object> source, ISpreadsheetData data)
        {
            foreach (var importer in source.OfType<ISpreadsheetAssetsExporter>().Where(x=>x.CanExport)) {
                importer.Export(data);
            }

            return data;
        }

        public sealed override IEnumerable<object> ImportObjects(IEnumerable<object> source,ISpreadsheetData spreadsheetData)
        {
            var result = new List<object>();
            
            foreach (var importer in OnPreImport(source.OfType<T>()).Where(x=>x.CanImport)) {
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