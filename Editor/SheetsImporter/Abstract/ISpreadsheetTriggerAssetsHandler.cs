namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using global::UniGame.Core.Runtime;
    using Editor;

    public interface ISpreadsheetTriggerAssetsHandler : IResetable
    {
        void                                   Initialize(IGoogleSpreadsheetClient client);
    }
}