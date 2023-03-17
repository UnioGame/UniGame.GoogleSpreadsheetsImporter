namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniModules.UniCore.Runtime.ReflectionUtils;
    using UniModules.UniGame.GoogleSpreadsheets.Editor.SheetsImporter;
    using UniModules.UniGame.GoogleSpreadsheets.Runtime.Attributes;

    public class SheetSyncSchemaProcessor
    {
        public SheetSyncScheme CreateSyncScheme(Type type)
        {
            var sheetItemAttribute = type.GetCustomAttribute<SpreadsheetTargetAttribute>();
            
            return CreateSyncScheme(type,sheetItemAttribute);
        }
        
        public SheetSyncScheme CreateSyncScheme(Type type,ISpreadsheetDescription spreadsheetTarget)
        {
            var sheetName    = type.Name;
            var useAllFields = true;
            var keyField     = GoogleSheetImporterConstants.KeyField;

            var sheetItemAttribute = spreadsheetTarget;
            if (sheetItemAttribute != null) {
                useAllFields = sheetItemAttribute.SyncAllFields;
                sheetName    = sheetItemAttribute.UseTypeName ? sheetName : sheetItemAttribute.SheetName;
                keyField     = sheetItemAttribute.KeyField;
            }

            var result = new SheetSyncScheme(sheetName);

            result.fields = LoadSyncFieldsData(type,keyField, useAllFields).ToArray();
            result.keyField = result.fields.FirstOrDefault(x => x.isKeyField);
            
            return result;

        }
        
        private IEnumerable<SyncField> LoadSyncFieldsData(Type sourceType,string keyField, bool useAllFields)
        {
            var fields = sourceType.GetInstanceFields();

            var spreadsheetTargetAttribute = sourceType.
                GetCustomAttribute<SpreadsheetTargetAttribute>();
            
            var filedsAttributes  = new List<SpreadSheetFieldAttribute>();
            var keyFieldSheetName = GoogleSheetImporterConstants.KeyField;
            
            var keyFieldName = spreadsheetTargetAttribute != null ? 
                spreadsheetTargetAttribute.KeyField :
                keyField;
            
            foreach (var field in fields) {
                var attributeInfo = field.
                    FieldType.
                    GetCustomAttribute<SpreadSheetFieldAttribute>();
                
                filedsAttributes.Add(attributeInfo);
                
                if (attributeInfo != null && attributeInfo.isKey)
                    keyFieldName = attributeInfo.useFieldName ? field.Name : attributeInfo.sheetField;
            }

            for (var i = 0; i < fields.Count; i++)
            {
                var fieldInfo       = fields[i];
                var customAttribute = filedsAttributes[i];
                if (customAttribute == null && !useAllFields)
                    continue;

                var fieldName = SheetData.FormatKey(fieldInfo.Name);
                var sheetField = customAttribute!=null && !customAttribute.useFieldName ? 
                    customAttribute.sheetField : fieldName;

                var isKeyField    = SheetData.IsEquals(keyFieldName, fieldName);
                var spreadsheetAttribute = fieldInfo.
                    GetCustomAttributes(typeof(SpreadsheetTargetAttribute), true).
                    FirstOrDefault() as ISpreadsheetDescription;
                var fieldSyncScheme = spreadsheetAttribute == null ? null : 
                    CreateSyncScheme(fieldInfo.FieldType, spreadsheetAttribute);
                
                var syncField  = new SyncField(fieldInfo, sheetField,isKeyField,fieldSyncScheme);

                yield return syncField;
            }
        }


    }
}