using System.Data;

namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.CoProcessors.Abstract
{
    public interface ICoProcessorHandle
    {
        void Apply(SheetValueInfo valueInfo, DataRow row);
    }
}