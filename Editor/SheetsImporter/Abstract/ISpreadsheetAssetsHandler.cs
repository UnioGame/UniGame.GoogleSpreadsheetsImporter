namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System.Collections.Generic;
    using UniModules.UniGame.GoogleSpreadsheetsImporter.Editor;

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