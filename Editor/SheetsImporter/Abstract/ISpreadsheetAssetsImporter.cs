namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System.Collections.Generic;
    using Editor;

    public interface ISpreadsheetAssetsImporter
    {
        bool CanImport { get; }

        IEnumerable<object> Import(ISpreadsheetData spreadsheetData);
    }
}