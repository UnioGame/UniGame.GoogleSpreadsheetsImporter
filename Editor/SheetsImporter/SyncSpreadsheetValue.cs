namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using UniModules.UniGame.GoogleSpreadsheets.Runtime.Attributes;

    public class SyncSpreadsheetValue : IEquatable<SyncSpreadsheetValue>
    {
        public Type type;
        public ISpreadsheetDescription spreadsheet;

        public bool Equals(SyncSpreadsheetValue other)
        {
            if(other == null) return false;
            if(type != other.type) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if(obj is SyncSpreadsheetValue value)
                return Equals(value);
            return false;
        }

        public override int GetHashCode()
        {
            return type == null ? 0 : type.GetHashCode();
        }
    }
}