namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System.Collections.Generic;

    public interface ISpreadsheetData
    {
        bool                   HasSheet(string sheetId);
        IEnumerable<SheetData> Sheets { get; }
        SheetData this[string sheetName] { get; }
    }
}