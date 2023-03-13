namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System.Collections.Generic;

    public interface ISpreadsheetData
    {
        bool                   HasSheet(string sheetName);
        IEnumerable<SheetData> Sheets { get; }
        SheetData this[string sheetName] { get; }
    }
}