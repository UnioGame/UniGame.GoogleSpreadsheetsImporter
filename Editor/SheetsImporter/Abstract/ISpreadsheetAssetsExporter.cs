namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System.Collections.Generic;
    using Editor;

    public interface ISpreadsheetAssetsExporter
    {
        bool CanExport { get; }
        
        ISpreadsheetData Export(ISpreadsheetData data);

        ISpreadsheetData ExportObjects(IEnumerable<object> source, ISpreadsheetData data);
    }
}