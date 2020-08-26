namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.Abstract
{
    using System;

    public interface ISpreadsheetTriggerAssetsHandler
    {
        IObservable<ISpreadsheetAssetsHandler> ImportCommand { get; }
        IObservable<ISpreadsheetAssetsHandler> ExportCommand { get; }
        void                                   Initialize();
    }
}