namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using UniRx;

    [Serializable]
    public abstract class SerializableSpreadsheetImporter : 
        ISpreadsheetAssetsHandler,
        ISpreadsheetTriggerAssetsHandler
    {
        private ISubject<ISpreadsheetAssetsHandler> _importCommand;
        private ISubject<ISpreadsheetAssetsHandler> _exportCommand;
        private IGoogleSpreadsheetClient            _client;
        private ISpreadsheetStatus                  _status;
        
        #region public properties
        
        public bool IsValidData => _importCommand != null && _exportCommand !=null && _status!=null && _status.HasConnectedSheets;

        public IObservable<ISpreadsheetAssetsHandler> ImportCommand => _importCommand;

        public IObservable<ISpreadsheetAssetsHandler> ExportCommand => _exportCommand;
        
        #endregion

        public void Initialize(IGoogleSpreadsheetClient client)
        {
            _client        = client;
            _status        = client.Status;
            _importCommand = new Subject<ISpreadsheetAssetsHandler>();
            _exportCommand = new Subject<ISpreadsheetAssetsHandler>();
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
        [Sirenix.OdinInspector.ButtonGroup()]
        [Sirenix.OdinInspector.Button()]
        [Sirenix.OdinInspector.ShowIf("IsValidData")]
#endif
        public void Import()
        {
            _importCommand?.OnNext(this);
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ButtonGroup()]
        [Sirenix.OdinInspector.Button()]
        [Sirenix.OdinInspector.ShowIf("IsValidData")]
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