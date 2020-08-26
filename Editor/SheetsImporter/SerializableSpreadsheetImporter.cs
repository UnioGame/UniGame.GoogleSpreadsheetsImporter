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
        
        #region public properties
        
        public bool IsValidData => _importCommand != null && _exportCommand !=null;

        public IObservable<ISpreadsheetAssetsHandler> ImportCommand => _importCommand;

        public IObservable<ISpreadsheetAssetsHandler> ExportCommand => _exportCommand;
        
        #endregion

        public void Initialize()
        {
            _importCommand = new Subject<ISpreadsheetAssetsHandler>();
            _exportCommand = new Subject<ISpreadsheetAssetsHandler>();
        }

        public virtual IEnumerable<object> Load()
        {
            yield break;
        }

        public IEnumerable<object> Import(SpreadsheetData spreadsheetData)
        {
            var source = Load();
            return ImportObjects(source, spreadsheetData);
        }

        public SpreadsheetData Export(SpreadsheetData data)
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


        public virtual IEnumerable<object> ImportObjects(IEnumerable<object> source,SpreadsheetData spreadsheetData)
        {
            return source;
        }

        public virtual SpreadsheetData ExportObjects(IEnumerable<object> source,SpreadsheetData spreadsheetData) => spreadsheetData;
    }
}