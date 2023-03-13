﻿using System;

namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    [Flags]
    public enum ImportAction
    {
        Import = 1,
        Export = 1 << 1,
        All = Import | Export
    }
}