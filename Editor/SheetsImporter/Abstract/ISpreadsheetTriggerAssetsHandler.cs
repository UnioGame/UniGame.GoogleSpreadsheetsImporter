namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.Abstract
{
    using System;
    using global::UniGame.Core.Runtime;

    public interface ISpreadsheetTriggerAssetsHandler : IResetable
    {
        IObservable<ISpreadsheetAssetsHandler> ImportCommand { get; }
        IObservable<ISpreadsheetAssetsHandler> ExportCommand { get; }
        
        void                                   Initialize(IGoogleSpreadsheetClient status);
    }
}