namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Runtime;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;
    using UniModules.UniCore.Runtime.DataFlow;
    using UnityEngine;
    
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public class SpreadsheetHandler : ISpreadsheetAssetsHandler, IResetable
    {
        private static Color _oddColor = new Color(0.2f, 0.4f, 0.3f);
        
        #region inspector

        public string importerName = string.Empty;

#if ODIN_INSPECTOR
        [InfoBox("Reload spreadsheet on each reimport action")]
#endif
        public bool autoReloadSpreadsheetOnImport = true;

#if ODIN_INSPECTOR
        [ListDrawerSettings(ElementColor = nameof(GetElementColor),ListElementLabelName = "Name")]
#endif
        public List<SpreadsheetImporterValue> importers = new List<SpreadsheetImporterValue>();

        #endregion

        #region private data

        private ISpreadsheetData _spreadsheetData;
        private IGoogleSpreadsheetClient _client;
        private LifeTimeDefinition _lifeTime;

        #endregion

        public string Name => string.IsNullOrEmpty(importerName) ? GetType().Name : importerName;

        public ILifeTime LifeTime => _lifeTime = _lifeTime ?? new LifeTimeDefinition();

        public IEnumerable<ISpreadsheetHandler> Importers => importers
            .Where(x => x.HasValue)
            .Select(x => x.Value);

        public bool CanImport => true;
        public bool CanExport => true;

        public void Reset() => _lifeTime?.Release();

        public void Initialize(IGoogleSpreadsheetClient client)
        {
            Reset();

            _client = client;
            _spreadsheetData = client.SpreadsheetData;

            foreach (var importer in Importers)
            {
                importer.Initialize(client);
            }

            LifeTime.AddCleanUpAction(() => _client = null);
            LifeTime.AddCleanUpAction(() => _spreadsheetData = null);
        }

        public IEnumerable<object> Load()
        {
            var result = new List<object>();
            foreach (var importer in Importers)
            {
                result.AddRange(importer.Load());
            }

            return result;
        }

        public IEnumerable<object> Import()
        {
            return Import(_spreadsheetData);
        }

        public ISpreadsheetData Export()
        {
            var data = Export(_spreadsheetData);
            return ExportSheets(data);
        }

        public ISpreadsheetData ExportSheets(ISpreadsheetData data)
        {
            var sheets = data.Sheets.ToList();
            foreach (var sheetData in sheets.Where(sheetData => sheetData.IsChanged))
            {
                _client.Upload(sheetData);
            }

            return data;
        }

        public IEnumerable<object> Import(ISpreadsheetData spreadsheetData)
        {
            if (autoReloadSpreadsheetOnImport)
                _client.ReloadAll();

            var result = new List<object>();
            foreach (var importer in Importers.Where(x => x.CanImport))
            {
                result.AddRange(Import(spreadsheetData, importer));
            }

            return result;
        }

        public IEnumerable<object> ImportObjects(IEnumerable<object> source, ISpreadsheetData spreadsheetData)
        {
            var stage = source;
            foreach (var importer in Importers.Where(x => x.CanImport))
            {
                stage = importer.ImportObjects(stage, spreadsheetData);
            }

            return stage;
        }

        public ISpreadsheetData ExportObjects(IEnumerable<object> source, ISpreadsheetData data)
        {
            var stage = data;
            var sourceData = source.ToList();
            
            foreach (var importer in Importers.Where(x => x.CanImport))
            {
                stage = importer.ExportObjects(sourceData, data);
            }

            return stage;
        }

        public ISpreadsheetData Export(ISpreadsheetData data)
        {
            foreach (var importer in Importers.Where(x => x.CanExport))
            {
                Export(data, importer);
            }

            return data;
        }
        
        public virtual string FormatName(string assetName) => assetName;

        private ISpreadsheetData Export(ISpreadsheetData data, ISpreadsheetAssetsHandler importer)
        {
            importer.Load();
            return importer.Export(data);
        }

        private IEnumerable<object> Import(ISpreadsheetData data, ISpreadsheetAssetsHandler importer)
        {
            importer.Load();
            return importer.Import(data);
        }

        public virtual void Start()
        {
        }

        
        private Color GetElementColor(int index, Color defaultColor)
        {
            var result = index % 2 == 0 
                ? _oddColor : defaultColor;
            return result;
        }
        
        private string GetImporterName(SpreadsheetImporterValue importer)
        {
            return importer.HasValue == false ? "EMPTY" : importer.Value.Name;
        }
    }
}