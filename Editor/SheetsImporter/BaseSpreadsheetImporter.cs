namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using UniRx;
    using UnityEngine;

    public abstract class BaseSpreadsheetImporter : ScriptableObject, 
        ISpreadsheetAssetsHandler,
        ISpreadsheetTriggerAssetsHandler
    {
        private Subject<ISpreadsheetAssetsHandler> _importCommand;
        private Subject<ISpreadsheetAssetsHandler> _exportCommand;
        private IGoogleSpreadsheetClient           _client;
        private IGooglsSpreadsheetClientStatus _status;

        #region public properties
        
        public bool IsValidData => _importCommand != null && _exportCommand !=null && _status!=null && _status.HasConnectedSheets;
        
        public abstract bool CanImport { get; }
        public abstract bool CanExport { get; }

        public IObservable<ISpreadsheetAssetsHandler> ImportCommand => _importCommand;

        public IObservable<ISpreadsheetAssetsHandler> ExportCommand => _exportCommand;
        
        #endregion

        public virtual void Initialize(IGoogleSpreadsheetClient client)
        {
            Reset();

            _client = client;
            _status = client.Status;

            _importCommand = new Subject<ISpreadsheetAssetsHandler>();
            _exportCommand = new Subject<ISpreadsheetAssetsHandler>();
        }

        public abstract IEnumerable<object> Load();

        public void Reset()
        {
            _client        = null;
            _status        = null;
            
            _importCommand?.Dispose();
            _exportCommand?.Dispose();
            
            _importCommand = null;
            _exportCommand = null;
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