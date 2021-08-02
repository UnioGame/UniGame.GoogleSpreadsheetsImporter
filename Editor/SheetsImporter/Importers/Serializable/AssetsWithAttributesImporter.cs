namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.Importers.Serializable
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniModules.Editor;
    using Extensions;
    using GoogleSpreadsheets.Runtime.Attributes;
    using UnityEngine;

    [Serializable]
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.HideLabel]
    [Sirenix.OdinInspector.BoxGroup("Attributes Source")]
#endif
    public class AssetsWithAttributesImporter :  SerializableSpreadsheetImporter 
    {
        
        /// <summary>
        /// list of assets linked by attributes
        /// </summary>
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.TableList]
        [Sirenix.OdinInspector.InlineButton("LoadAssets")]
#endif
        public List<SheetSyncItem> assets = new List<SheetSyncItem>();

        public void LoadAssets()
        {
            foreach (var asset in Load()) {
                
            }
        }
        
        public override IEnumerable<object> Load()
        {
            var attributeAssets = AssetEditorTools.
                GetAssetsWithAttribute<ScriptableObject,SpreadsheetTargetAttribute>();

            assets = attributeAssets.
                Select(x=> new SheetSyncItem() {
                    asset = x.Value,
                    sheetId = x.Attribute == null || x.Attribute.UseTypeName ?
                        x.Value.GetType().Name : 
                        x.Attribute.SheetName
                }).
                ToList();
            
            return assets.
                OfType<object>().
                ToList();
        }

        public override IEnumerable<object> ImportObjects(IEnumerable<object> source,ISpreadsheetData spreadsheetData)
        {
            var result = new List<object>();
            foreach (var item in source.OfType<SheetSyncItem>()) {
                if(!spreadsheetData.HasSheet(item.sheetId) || item.asset == null)
                    continue;
                var asset = item.asset.
                    ApplySpreadsheetData(spreadsheetData,item.sheetId);
                result.Add(asset);
            }

            return result;
        }
        
        public override ISpreadsheetData ExportObjects(IEnumerable<object> source,ISpreadsheetData spreadsheetData)
        {
            foreach (var item in assets) {
                if(item.asset == null)
                    continue;
                item.asset.UpdateSheetValue(spreadsheetData, item.sheetId);
            }
            return spreadsheetData;
        }
    }
}