namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Runtime.SerializableType;
    using Core.Runtime.SerializableType.Extensions;
    using UniModules.Editor;
    using UniModules.UniGame.GoogleSpreadsheets.Runtime.Attributes;
    using UnityEngine;
    using Object = UnityEngine.Object;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
    //public class AssetsWithAttributesImporterAsset  : BaseSpreadsheetImporter
    
    [Serializable]
#if ODIN_INSPECTOR
    [HideLabel]
    [BoxGroup("Attributes Source")]
#endif
    public class AssetsWithAttributesImporter :  SerializableSpreadsheetImporter 
    {
#if ODIN_INSPECTOR
        [ValueDropdown(nameof(GetAssetTypeDropdown))]
        [PropertyOrder(-1)]
#endif
        public SType targetAssetType = typeof(ScriptableObject);

        /// <summary>
        /// list of assets linked by attributes
        /// </summary>
#if ODIN_INSPECTOR
        [TableList]
#endif
        public List<SheetSyncItem> assets = new List<SheetSyncItem>();

        [Button]
        public void LoadAssets()
        {
            foreach (var asset in Load()) {
                
            }
        }
        
        public override IEnumerable<object> Load()
        {
            if(targetAssetType == null)yield break;
            
            yield return AssetEditorTools.GetAssets(targetAssetType);
        }

        public override IEnumerable<object> ImportObjects(IEnumerable<object> source,ISpreadsheetData spreadsheetData)
        {
            assets.Clear();
            var resultObjects = new List<object>();
            var result = FilterAttributesTargets(source);
            
            foreach (var item in result) {
                if(!spreadsheetData.HasSheet(item.sheetId) || item.asset == null)
                    continue;
                var resultValue = item.target.ApplySpreadsheetData(spreadsheetData,item.sheetId);
                resultObjects.Add(item.asset);
                
                assets.Add(item);
                
                if (item.asset != null)
                    item.asset.MarkDirty();
            }

            return resultObjects;
        }
        
        public override ISpreadsheetData ExportObjects(IEnumerable<object> source,ISpreadsheetData spreadsheetData)
        {
            foreach (var item in assets) {
                if(item.asset == null) continue;
                item.asset.UpdateSheetValue(spreadsheetData, item.sheetId);
            }
            
            return spreadsheetData;
        }

        public IEnumerable<SheetSyncItem> FilterAttributesTargets(IEnumerable<object> source)
        {
            foreach (var sourceValue in source)
            {
                var attributeValue = sourceValue
                    .GetAttributeOnSelfOrChildren<SpreadsheetTargetAttribute>();
                
                if(!attributeValue.IsFound) continue;
                
                var attribute = attributeValue.Attribute;
                var target = attributeValue.Target;
                
                var sheetName = attribute == null || attribute.UseTypeName
                    ? target.GetType().Name
                    : attribute.SheetName;
                
                var asset = attributeValue.Source as Object;
                    
                var value = new SheetSyncItem
                {
                    asset = asset,
                    target = attributeValue.Target,
                    sheetId = sheetName
                };

                yield return value;
            }
        }
        
        private IEnumerable<ValueDropdownItem<SType>> GetAssetTypeDropdown()
        {
            var baseType = typeof(ScriptableObject);
            var baseSType = (SType)baseType;
            var assetTypes = baseSType.GetAssignableNonAbstractTypes();
            foreach (var sType in assetTypes)
            {
                yield return new ValueDropdownItem<SType>()
                {
                    Text = sType.Name,
                    Value = sType
                };
            }
        }
    }
}