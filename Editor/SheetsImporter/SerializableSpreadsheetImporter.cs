namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public abstract class SerializableSpreadsheetImporter : ISpreadsheetHandler
    {
        public string importerName = string.Empty;
        
        [SerializeField]
        private ImportAction _importAction = ImportAction.All;
        
        private IGoogleSpreadsheetClient       _client;
        private IGooglsSpreadsheetClientStatus _status;

        #region public properties
        
        public virtual string Name => string.IsNullOrEmpty(importerName) ? GetType().Name : importerName;
        
        public bool CanImport => _importAction.HasFlag(ImportAction.Import);
        public bool CanExport => _importAction.HasFlag(ImportAction.Export);
        
        public bool IsValidData =>  _client!= null && 
                                    _status !=null && 
                                    _status.HasConnectedSheets;

        #endregion

        public void Initialize(IGoogleSpreadsheetClient client)
        {
            Reset();

            _client        = client;
            _status        = client.Status;
        }

        public void Reset()
        {
            _client        = null;
            _status        = null;
        }
        
        public virtual IEnumerable<object> Load()
        {
            yield break;
        }

        public IEnumerable<object> Import(ISpreadsheetData spreadsheetData)
        {
            var source = Load();
            var result = ImportObjects(source, spreadsheetData);
            foreach (var item in result)
                yield return item; 
        }

        public ISpreadsheetData Export(ISpreadsheetData data)
        {
            var source = Load();
            return ExportObjects(source, data);
        }

#if ODIN_INSPECTOR
        [ButtonGroup]
        [Button(ButtonSizes.Small,Icon = SdfIconType.CloudDownload)]
        [EnableIf(nameof(IsValidData))]
        [ShowIf(nameof(CanImport))]
#endif
        public void Import()
        {
            if (IsValidData == false) return;
            Import(_client.SpreadsheetData).ToList();
        }

#if ODIN_INSPECTOR
        [ButtonGroup]
        [Button(ButtonSizes.Small,Icon = SdfIconType.CloudUpload)]
        [EnableIf("IsValidData")]
        [ShowIf("CanExport")]
#endif
        public void Export()
        {
            if (IsValidData == false) return;
            Export(_client.SpreadsheetData);
        }
        
        public virtual void Start() { }
        
        public virtual IEnumerable<object> ImportObjects(IEnumerable<object> source,ISpreadsheetData spreadsheetData)
        {
            return source;
        }

        public virtual ISpreadsheetData ExportObjects(IEnumerable<object> source,ISpreadsheetData spreadsheetData) => spreadsheetData;

        public virtual string FormatName(string assetName) => assetName;
    }
}