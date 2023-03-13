namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using Editor;

    public interface ISpreadsheetAssetsExporter
    {
        bool CanExport { get; }
        
        ISpreadsheetData Export(ISpreadsheetData data);
    }
}