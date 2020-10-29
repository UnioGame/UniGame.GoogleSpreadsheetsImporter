namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.Abstract
{
    public interface ISpreadsheetAssetsExporter
    {
        bool CanExport { get; }
        
        ISpreadsheetData Export(ISpreadsheetData data);
    }
}