namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using UniModules.UniGame.GoogleSpreadsheets.Editor.SheetsImporter;

    [Serializable]
    public class SheetId
    {
        public string sheetName = string.Empty;
        public string keyField = GoogleSpreadsheetConstants.KeyField;
    }
}