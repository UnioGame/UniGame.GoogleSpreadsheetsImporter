namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstract;
    using Core.Runtime.DataFlow.Interfaces;
    using Importers.Serializable;
    using UniGreenModules.UniCore.Runtime.DataFlow;
    using UniGreenModules.UniCore.Runtime.Interfaces;
    using UniGreenModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class SpreadsheetImportersHandler : ISpreadsheetAssetsHandler,IResetable
    {
        
#region inspector

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
#endif
        [SerializeReference]
        public List<SerializableSpreadsheetImporter> serializabledImporters = 
            new List<SerializableSpreadsheetImporter>() {
                new AssetsWithAttributesImporter()
            };

        /// <summary>
        /// list of assets linked by attributes
        /// </summary>
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor(Sirenix.OdinInspector.InlineEditorModes.GUIOnly,
            Sirenix.OdinInspector.InlineEditorObjectFieldModes.Foldout)]
#endif
        public List<BaseSpreadsheetImporter> importers = new List<BaseSpreadsheetImporter>();

#endregion

#region private data

        private SpreadsheetData               _spreadsheetData;
        private List<GoogleSpreadsheetClient> _clients;
        private LifeTimeDefinition _lifeTime;

#endregion

        public ILifeTime LifeTime => _lifeTime = (_lifeTime ?? new LifeTimeDefinition());

        public IEnumerable<ISpreadsheetAssetsHandler> Importers => importers.
            Concat<ISpreadsheetAssetsHandler>(serializabledImporters);

        public void Reset() => _lifeTime?.Release();
        
        public void Initialize(SpreadsheetData spreadsheetData,IReadOnlyList<GoogleSpreadsheetClient> clients)
        {
            Reset();
            
            _spreadsheetData = spreadsheetData;
            _clients = new List<GoogleSpreadsheetClient>(clients);
            
            foreach (var importer in importers.
                Concat<ISpreadsheetTriggerAssetsHandler>(serializabledImporters)) 
            {
                importer.Initialize();
                importer.ExportCommand.Do(
                        x => ExportSheets(Export(_spreadsheetData, x))).
                    Subscribe().
                    AddTo(LifeTime);
                importer.ImportCommand.Do(
                        x => Import(_spreadsheetData,x)).
                    Subscribe().
                    AddTo(LifeTime);
            }

            LifeTime.AddCleanUpAction(() => _clients = null);
            LifeTime.AddCleanUpAction(() => _spreadsheetData = null);
        }
        
        public IEnumerable<object> Load()
        {
            var result = new List<object>();
            foreach (var importer in Importers) {
                result.AddRange(importer.Load());
            }

            return result;
        }

        public IEnumerable<object> Import()
        {
            return Import(_spreadsheetData);
        }
        
        public SpreadsheetData Export()
        {
            var data = Export(_spreadsheetData);
            return ExportSheets(data);
        }

        public SpreadsheetData ExportSheets(SpreadsheetData data)
        {
            foreach (var sheetData in data.Sheets) {
                if(!sheetData.IsChanged)
                    continue;
                foreach (var client in _clients) {
                    if(!client.HasSheet(sheetData.Id))
                        continue;
                    client.UpdateData(sheetData);
                }
            }

            return data;
        }
        
        
        
        public IEnumerable<object> Import(SpreadsheetData spreadsheetData)
        {
            var result = new List<object>();
            foreach (var importer in Importers) {
                result.AddRange(Import(spreadsheetData,importer));
            }
            return result;
        }

        public SpreadsheetData Export(SpreadsheetData data)
        {
            foreach (var importer in importers) {
                Export(data, importer);
            }
            return data;
        }

        private SpreadsheetData Export(SpreadsheetData data, ISpreadsheetAssetsHandler importer)
        {
            importer.Load();
            return importer.Export(data);
        }
        
        private IEnumerable<object> Import(SpreadsheetData data, ISpreadsheetAssetsHandler importer)
        {
            importer.Load();
            return importer.Import(data);
        }
    }
}