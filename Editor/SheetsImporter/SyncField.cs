namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System;
    using System.Reflection;
    
    [Serializable]
    public class SyncField
    {
        private FieldInfo _fieldInfo;
        
        public readonly Type            targetType;
        public readonly string          objectField;
        public readonly string          sheetField;
        public readonly bool            isKeyField;
        public readonly SheetSyncScheme syncScheme;
        
        public SyncField(FieldInfo field, string sheetValueField, bool isKeyField, SheetSyncScheme fieldScheme = null)
        {
            _fieldInfo       = field;
            objectField = _fieldInfo.Name;
            sheetField  = sheetValueField.TrimStart('_');
            this.isKeyField  = isKeyField;
            targetType  = _fieldInfo.FieldType;
            syncScheme  = fieldScheme;
        }

        public bool IsSheetTarget => syncScheme != null;
        
        #region public methods

        public object GetValue(object source)
        {
            return _fieldInfo.GetValue(source);
        }

        public SyncField ApplyValue(object source, object value)
        {
            _fieldInfo.SetValue(source,value);
            return this;
        }

        public override string ToString()
        {
            return $"Object Field: {objectField} IsKey: {isKeyField} Sheet Field: {sheetField} TargetType: {targetType.Name}";
        }
        
        #endregion
    }
}