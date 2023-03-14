namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [CreateAssetMenu(menuName = "UniGame/Google/Importers/PipelineImporter",fileName = nameof(PipelineImporter))]
    public class PipelineImporter : BaseSpreadsheetImporter
    {
        private static Color _oddColor = new Color(0.2f, 0.4f, 0.3f);
        
#if ODIN_INSPECTOR
        [ListDrawerSettings(ElementColor = nameof(GetElementColor),ListElementLabelName = "Name")]
#endif
        public List<SpreadsheetImporterValue> importers = new List<SpreadsheetImporterValue>();

        public override bool CanImport => importers
            .Where(x => x.HasValue)
            .Any(x => x.Value.CanImport);
        
        public override bool CanExport => importers
            .Where(x => x.HasValue)
            .Any(x => x.Value.CanExport);

        public override IEnumerable<object> Load()
        {
            var importer = importers
                .FirstOrDefault(x => x.HasValue);
            
            if(importer == null) yield break;

            var items = importer.Value.Load();
            foreach (var item in items)
                yield return item;
        }

        public override ISpreadsheetData ExportObjects(IEnumerable<object> source, ISpreadsheetData data)
        {
            var items = importers
                .Where(x => x.HasValue && x.Value.CanExport);
            
            foreach (var exporter in items) 
                exporter.Value.Export(data);

            return data;
        }

        public sealed override IEnumerable<object> ImportObjects(IEnumerable<object> source,ISpreadsheetData spreadsheetData)
        {
            var result = new List<object>();

            var items = OnPreImport(source);
            var stage = items;
            
            foreach (var importer in importers.Where(x => x.HasValue))
            {
                var value = importer.Value;
                stage = value.ImportObjects(stage,spreadsheetData);
            }

            stage = OnPostImport(stage).ToList();
            
            result.AddRange(stage);
            return result;
        }

        
        protected override void OnInitialize(IGoogleSpreadsheetClient client)
        {
            base.OnInitialize(client);

            foreach (var importer in importers.Where(x => x.HasValue))
            {
                var value = importer.Value;
                value.Initialize(client);
            }
        }

        
        protected virtual IEnumerable<object> OnPreImport(IEnumerable<object> sourceImporters)
        {
            return sourceImporters;
        }
        
        protected virtual IEnumerable<object> OnPostImport(IEnumerable<object> importedAssets)
        {
            return importedAssets;
        }
        
        private Color GetElementColor(int index, Color defaultColor)
        {
            var result = index % 2 == 0 
                ? _oddColor : defaultColor;
            return result;
        }
    }
}