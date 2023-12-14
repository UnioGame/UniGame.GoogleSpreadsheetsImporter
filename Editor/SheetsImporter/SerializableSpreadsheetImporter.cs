namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UniCore.Runtime.ProfilerTools;
    using UnityEditor;
    using UnityEngine;
    using Object = UnityEngine.Object;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    [Serializable]
    public abstract class SerializableSpreadsheetImporter : ISpreadsheetHandler
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
        [Button(ButtonSizes.Medium,Icon = SdfIconType.CloudDownload)]
        [EnableIf(nameof(IsValidData))]
        [ShowIf(nameof(CanImport))]
#endif
        public void Import()
        {
            if (IsValidData == false) return;
            
            var counter = 0;
            var stringBuilder = new StringBuilder();

            if (disableDBOnImport)
                AssetDatabase.StartAssetEditing();

            try
            {
                foreach (var importedObject in Import(_client.SpreadsheetData))
                {
                    var assetName = importedObject is Object asset ? asset.name : importedObject.GetType().Name;
                    stringBuilder.AppendLine($"{counter} : [{assetName}] : {importedObject}");
                    counter++;
                }
            }
            catch (Exception e)
            {
                GameLog.LogError(e);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
            
            stringBuilder.AppendLine($"\nImported {counter} objects");
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