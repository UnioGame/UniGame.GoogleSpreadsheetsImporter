namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using System.Collections.Generic;
    using Editor;

    [Serializable]
    public abstract class SpreadsheetSerializableImporter : ISpreadsheetAssetsHandler
    {
        public string importerName = string.Empty;
        
        public string Name => string.IsNullOrEmpty(importerName) ? GetType().Name : importerName;
        
        public abstract bool CanImport { get; }
        public abstract bool CanExport { get; }

        public abstract IEnumerable<object> Import(ISpreadsheetData spreadsheetData);

        public abstract IEnumerable<object> Load();

        public virtual ISpreadsheetData Export(ISpreadsheetData data)
        {
            return data;
        }

        public virtual void Start() { }

        public virtual string FormatName(string assetName) => assetName;
    }
}