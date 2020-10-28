namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.CoProcessors.Abstract
{
    public interface ICoProcessorHandle
    {
        bool CanApply(string columnName);
        void Apply(string columnName);
    }
}