namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using UniModules.UniGame.Core.Runtime.Common;

    [Serializable]
    public class SpreadsheetImporterValue : VariantValue<
        SerializableSpreadsheetImporter,
        BaseSpreadsheetProcessor,
        ISpreadsheetProcessor>
    {
        public const string EmptyValue = "EMPTY";
        
        public string Name => HasValue ? Value.Name : EmptyValue;
        
    }
}