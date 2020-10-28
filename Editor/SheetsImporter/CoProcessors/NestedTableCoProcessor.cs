namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.CoProcessors
{
    using System;
    using System.Text.RegularExpressions;
    using UnityEngine;
    
    [Serializable]
    [CreateAssetMenu(menuName = "UniGame/Google/CoProcessors/NestedTableCoProcessor", fileName = nameof(NestedTableCoProcessor))]
    public class NestedTableCoProcessor : BaseCoProcessor
    {
        private const int TableNameGroupId = 1;
        
        [SerializeField]
        private string _filter;
        
        public override bool CanApply(string columnName)
        {
            var regex = new Regex(_filter);
            return regex.IsMatch(columnName);
        }

        public override void Apply(string columnName)
        {
            
        }

        private string GetTableName(string columnName)
        {
            var regex = new Regex(_filter);
            if (!regex.IsMatch(columnName))
                return columnName;

            var groups = regex.Match(columnName).Groups;
            return groups[TableNameGroupId].Value;
        }
    }
}