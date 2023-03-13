using System.Data;

namespace UniGame.GoogleSpreadsheetsImporter.Editor.CoProcessors.Abstract
{
    public interface ICoProcessorHandle
    {
        void Apply(SheetValueInfo valueInfo, DataRow row);
    }
}