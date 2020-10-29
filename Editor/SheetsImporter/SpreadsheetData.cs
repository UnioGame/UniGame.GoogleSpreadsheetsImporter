namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [Serializable]
    public class SpreadsheetData : ISpreadsheetData
    {
        private IEnumerable<SheetData> _sheets;

        public SpreadsheetData(IEnumerable<SheetData> sheets)
        {
            _sheets = sheets;
        }

        public bool HasSheet(string sheetName) => _sheets.Any(x => x.Name.ToLower().Equals(sheetName.ToLower()));

        public IEnumerable<SheetData> Sheets => _sheets;

        public SheetData this[string sheetName] => _sheets.FirstOrDefault(x => x.Name.ToLower().Equals(sheetName.ToLower()));
    }
}