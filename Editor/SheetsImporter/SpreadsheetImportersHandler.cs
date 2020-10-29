namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstract;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using Importers.Serializable;
    using UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class SpreadsheetImportersHandler : ISpreadsheetAssetsHandler, IResetable
    {
        #region inspector

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InfoBox("Reload spreadsheet on each reimport action")]
#endif
        public bool autoReloadSpreadsheetOnImport = true;

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

        private ISpreadsheetData         _spreadsheetData;
        private IGoogleSpreadsheetClient _client;
        private LifeTimeDefinition       _lifeTime;

        #endregion

        public ILifeTime LifeTime => _lifeTime = (_lifeTime ?? new LifeTimeDefinition());

        public IEnumerable<ISpreadsheetAssetsHandler> Importers => importers.Concat<ISpreadsheetAssetsHandler>(serializabledImporters);

        public void Reset() => _lifeTime?.Release();

        public void Initialize(IGoogleSpreadsheetClient client)
        {
            Reset();

            _client          = client;
            _spreadsheetData = client.SpreadsheetData;

            foreach (var importer in importers.Concat<ISpreadsheetTriggerAssetsHandler>(serializabledImporters)) {
                importer.Initialize(client);
                importer.ExportCommand.
                    Do(x => ExportSheets(Export(_spreadsheetData, x))).
                    Subscribe().
                    AddTo(LifeTime);

                importer.ImportCommand.
                    Do(x => {
                        if (autoReloadSpreadsheetOnImport)
                            _client.ReloadAll();
                    }).
                    Do(x => Import(_spreadsheetData, x)).
                    Subscribe().
                    AddTo(LifeTime);
            }

            LifeTime.AddCleanUpAction(() => _client          = null);
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

        public ISpreadsheetData Export()
        {
            var data = Export(_spreadsheetData);
            return ExportSheets(data);
        }

        public ISpreadsheetData ExportSheets(ISpreadsheetData data)
        {
            var sheets = data.Sheets.ToList();
            foreach (var sheetData in sheets.Where(sheetData => sheetData.IsChanged)) {
                _client.Upload(sheetData);
            }

            return data;
        }

        public IEnumerable<object> Import(ISpreadsheetData spreadsheetData)
        {
            if (autoReloadSpreadsheetOnImport)
                _client.ReloadAll();
            
            var result = new List<object>();
            foreach (var importer in Importers) {
                result.AddRange(Import(spreadsheetData, importer));
            }

            return result;
        }

        public ISpreadsheetData Export(ISpreadsheetData data)
        {
            foreach (var importer in importers) {
                Export(data, importer);
            }

            return data;
        }

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
    }
}