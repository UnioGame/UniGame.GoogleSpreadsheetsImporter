﻿namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using UniModules.UniGame.GoogleSpreadsheetsImporter.Editor;

    public interface ISpreadsheetAssetsProcessor : 
        ISpreadsheetAssetsImporter,
        ISpreadsheetAssetsExporter,
        IStartable,
        IAssetNameFormatter
    {
        public string Name { get; }
    }
}