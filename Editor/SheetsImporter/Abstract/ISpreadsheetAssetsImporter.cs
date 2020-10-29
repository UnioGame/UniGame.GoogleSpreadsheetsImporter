namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.Abstract
{
    using System.Collections.Generic;

    public interface ISpreadsheetAssetsImporter
    {
        bool CanImport { get; }

        IEnumerable<object> Import(ISpreadsheetData spreadsheetData);
    }
}