namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class SheetSyncScheme
    {
        public object target;

        public string sheetId;

        public SyncField keyField;
        
        public SyncField[] fields = Array.Empty<SyncField>();

        public SheetSyncScheme(string sheetId)
        {
            this.sheetId = sheetId;
        }

        public SyncField GetFieldBySheetFieldName(string fieldName)
        {
            return fields.FirstOrDefault(x => SheetData.IsEquals(x.sheetField, fieldName));
        }
        
        public SyncField GetFieldByObjectField(string fieldName)
        {
            return fields.FirstOrDefault(x => SheetData.IsEquals(x.objectField, fieldName));
        }
    }
}