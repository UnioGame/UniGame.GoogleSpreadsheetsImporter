using System;
using System.Data;
using UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.CoProcessors.Abstract;

namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.CoProcessors
{
    [Serializable]
    public abstract class BaseCoProcessor : ICoProcessorHandle
    {
        public abstract void Apply(SheetValueInfo sheetValueInfo, DataRow row);
    }
}