namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using global::UniGame.Core.Runtime;
    using Editor;

    public interface ISpreadsheetTriggerAssetsHandler : IResetable
    {
        IObservable<ISpreadsheetAssetsHandler> ImportCommand { get; }
        IObservable<ISpreadsheetAssetsHandler> ExportCommand { get; }
        
        void                                   Initialize(IGoogleSpreadsheetClient status);
    }
}