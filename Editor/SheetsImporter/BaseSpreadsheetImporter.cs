namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using System.Collections.Generic;
    using Core.Runtime;
    using UniModules.UniCore.Runtime.DataFlow;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif

    public abstract class BaseSpreadsheetImporter : ScriptableObject, ISpreadsheetHandler
    {
        public string importerName = string.Empty;

        private IGoogleSpreadsheetClient _client;
        private IGooglsSpreadsheetClientStatus _status;
        
        private LifeTimeDefinition _lifeTimeDefinition = new LifeTimeDefinition();

        #region public properties

        public virtual string Name => string.IsNullOrEmpty(importerName) ? name : importerName;

        protected ILifeTime LifeTime => _lifeTimeDefinition;

        public IGoogleSpreadsheetClient Client => _client;

        public bool IsValidData =>  _client!=null && 
                                    _status != null && 
                                    _status.HasConnectedSheets;

        public abstract bool CanImport { get; }
        public abstract bool CanExport { get; }

        #endregion

        public virtual void Start()
        {
        }

        public virtual void Initialize(IGoogleSpreadsheetClient client)
        {
            Reset();

            _client = client;
            _status = client.Status;

            OnInitialize(client);
        }

        public abstract IEnumerable<object> Load();

        public void Reset()
        {
            _client = null;
            _status = null;

            _lifeTimeDefinition.Terminate();
            _lifeTimeDefinition = new LifeTimeDefinition();
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
        [ButtonGroup]
        [Button(ButtonSizes.Small,Icon = SdfIconType.CloudDownload)]
        [EnableIf(nameof(IsValidData))]
        [ShowIf(nameof(CanImport))]
#endif
        public void Import()
        {
            if (IsValidData == false) return;
            Import(_client.SpreadsheetData);
        }

#if ODIN_INSPECTOR
        [ButtonGroup]
        [Button(ButtonSizes.Small,Icon = SdfIconType.CloudUpload)]
        [EnableIf(nameof(IsValidData))]
        [ShowIf(nameof(CanExport))]
#endif
        public void Export()
        {
            if (IsValidData == false) return;
            Export(_client.SpreadsheetData);
        }

        public abstract IEnumerable<object> ImportObjects(IEnumerable<object> source, ISpreadsheetData spreadsheetData);

        public abstract ISpreadsheetData ExportObjects(IEnumerable<object> source, ISpreadsheetData spreadsheetData);

        public virtual string FormatName(string assetName) => assetName;
        
        protected virtual void OnReset() {}
        
        protected virtual void OnInitialize(IGoogleSpreadsheetClient client) {}
    }
}