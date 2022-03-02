namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.Abstract
{
    using System.Collections.Generic;

    public interface ISpreadsheetAssetsHandler : 
        ISpreadsheetAssetsImporter,
        ISpreadsheetAssetsExporter,
        IStartable,
        IAssetNameFormatter
    {
        public string Name { get; }
        
        IEnumerable<object> Load();
    }
}