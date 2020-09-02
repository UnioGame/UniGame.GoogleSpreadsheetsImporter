namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.Importers
{
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Object = UnityEngine.Object;

    [Serializable]
    public abstract class SpreadsheetSerializableImporter : ISpreadsheetAssetsHandler
    {
        public abstract IEnumerable<object> Import(ISpreadsheetData spreadsheetData);

        public abstract IEnumerable<object> Load();

        public virtual ISpreadsheetData Export(ISpreadsheetData data)
        {
            return data;
        }
    }
}