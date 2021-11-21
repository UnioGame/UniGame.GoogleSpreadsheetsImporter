namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public abstract class SerializableSpreadsheetImporter : 
        ISpreadsheetAssetsHandler,
        ISpreadsheetTriggerAssetsHandler
    {
        public string importerName = string.Empty;
        
        [SerializeField]
        private ImportAction _importAction = ImportAction.All;
        
        
        private Subject<ISpreadsheetAssetsHandler> _importCommand;
        private Subject<ISpreadsheetAssetsHandler> _exportCommand;
        private IGoogleSpreadsheetClient           _client;
        private IGooglsSpreadsheetClientStatus _status;

        #region public properties
        
        public string Name => string.IsNullOrEmpty(importerName) ? GetType().Name : importerName;
        public bool CanImport => _importAction.HasFlag(ImportAction.Import);
        public bool CanExport => _importAction.HasFlag(ImportAction.Export);
        
        public bool IsValidData => _importCommand != null && _exportCommand !=null && _status !=null && _status.HasConnectedSheets;
        
        public IObservable<ISpreadsheetAssetsHandler> ImportCommand => _importCommand;

        public IObservable<ISpreadsheetAssetsHandler> ExportCommand => _exportCommand;
        
        #endregion

        public void Initialize(IGoogleSpreadsheetClient client)
        {
            Reset();

            _client        = client;
            _status        = client.Status;
            _importCommand = new Subject<ISpreadsheetAssetsHandler>();
            _exportCommand = new Subject<ISpreadsheetAssetsHandler>();
        }

        public void Reset()
        {
            _client        = null;
            _status        = null;
            
            _importCommand?.Dispose();
            _exportCommand?.Dispose();

            _importCommand = null;
            _exportCommand = null;
        }
        
        public virtual IEnumerable<object> Load()
        {
            yield break;
        }

        public IEnumerable<object> Import(ISpreadsheetData spreadsheetData)
        {
            var source = Load();
            return ImportObjects(source, spreadsheetData);
        }

        public ISpreadsheetData Export(ISpreadsheetData data)
        {
            var source = Load();
            return ExportObjects(source, data);
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ButtonGroup]
        [Sirenix.OdinInspector.Button]
        [Sirenix.OdinInspector.EnableIf("IsValidData")]
        [Sirenix.OdinInspector.EnableIf("CanImport")]
#endif
        public void Import()
        {
            _importCommand?.OnNext(this);
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ButtonGroup]
        [Sirenix.OdinInspector.Button]
        [Sirenix.OdinInspector.EnableIf("IsValidData")]
        [Sirenix.OdinInspector.EnableIf("CanExport")]
#endif
        public void Export()
        {
            _exportCommand?.OnNext(this);
        }
        
        public virtual IEnumerable<object> ImportObjects(IEnumerable<object> source,ISpreadsheetData spreadsheetData)
        {
            return source;
        }

        public virtual ISpreadsheetData ExportObjects(IEnumerable<object> source,ISpreadsheetData spreadsheetData) => spreadsheetData;
    }
}