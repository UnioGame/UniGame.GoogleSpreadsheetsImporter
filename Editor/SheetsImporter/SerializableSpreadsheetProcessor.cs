﻿namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using System.Text;
    using UniCore.Runtime.ProfilerTools;
    using UnityEditor;
    using UnityEngine;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public abstract class SerializableSpreadsheetProcessor : ISpreadsheetProcessor
    {
        public string importerName = string.Empty;
        
        public bool disableDBOnImport = false;
        
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

        public ISpreadsheetData Import(ISpreadsheetData spreadsheetData)
        {
            return ImportObjects(spreadsheetData);
        }

        public ISpreadsheetData Export(ISpreadsheetData data)
        {
            return ExportObjects(data);
        }

#if ODIN_INSPECTOR
        [ButtonGroup]
        [Button(ButtonSizes.Medium,Icon = SdfIconType.CloudDownload)]
        [EnableIf(nameof(IsValidData))]
        [ShowIf(nameof(CanImport))]
#endif
        public void Import()
        {
            if (IsValidData == false) return;
            
            var stringBuilder = new StringBuilder();

            if (disableDBOnImport)
                AssetDatabase.StartAssetEditing();

            try
            {
                Import(_client.SpreadsheetData);
            }
            catch (Exception e)
            {
                GameLog.LogError(e);
                return;
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
            
            GameLog.Log(stringBuilder.ToString());
        }

#if ODIN_INSPECTOR
        [ButtonGroup]
        [Button(ButtonSizes.Medium,Icon = SdfIconType.CloudUpload)]
        [EnableIf("IsValidData")]
        [ShowIf("CanExport")]
#endif
        public void Export()
        {
            if (IsValidData == false) return;
            Export(_client.SpreadsheetData);
            
            if(_client.IsConnected) _client.UploadAll();
        }
        
        public virtual void Start() { }
        
        public virtual ISpreadsheetData ImportObjects(ISpreadsheetData spreadsheetData)
        {
            return spreadsheetData;
        }

        public virtual ISpreadsheetData ExportObjects(ISpreadsheetData spreadsheetData) => spreadsheetData;

        public virtual string FormatName(string assetName) => assetName;
    }
}