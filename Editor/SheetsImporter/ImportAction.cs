using System;

namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    [Flags]
    public enum ImportAction
    {
        Import = 1,
        Export = 1 << 1,
        All = Import | Export
    }
}