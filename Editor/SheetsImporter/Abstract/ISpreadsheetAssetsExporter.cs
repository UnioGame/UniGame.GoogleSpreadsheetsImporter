namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.Abstract
{
    public interface ISpreadsheetAssetsExporter
    {
        ISpreadsheetData Export(ISpreadsheetData data);
    }
}