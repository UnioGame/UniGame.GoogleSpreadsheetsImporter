namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System.Collections.Generic;

    public class SheetValueInfo
    {
        public object          Source;
        public SheetSyncScheme SyncScheme;
        public SpreadsheetData SpreadsheetData;
        public string          SheetId;
        public string          SyncFieldName;
        public object          SyncFieldValue;
        public HashSet<object> IgnoreCache;
        
    }
}