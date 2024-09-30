namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    public interface ISpreadsheetAssetsExporter
    {
        bool CanExport { get; }
        
        ISpreadsheetData Export(ISpreadsheetData data);

    }
}