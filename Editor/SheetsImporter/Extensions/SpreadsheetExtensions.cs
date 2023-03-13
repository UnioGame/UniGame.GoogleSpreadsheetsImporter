﻿using System.Data;
using System.Linq;
using UniGame.GoogleSpreadsheetsImporter.Editor.CoProcessors;

namespace UniGame.GoogleSpreadsheetsImporter.Editor
{
    using System;
    using System.Collections.Generic;
    using Editor;
    using UniModules.UniGame.TypeConverters.Editor;
    using Object = UnityEngine.Object;

    public static class SpreadsheetExtensions
    {
        public static readonly AssetSheetDataProcessor DefaultProcessor = new AssetSheetDataProcessor(CoProcessor.Processor);

        public static bool UpdateSheetValue(this object source, ISpreadsheetData data, string sheetId, string sheetKeyField)
        {
            return DefaultProcessor.UpdateSheetValue(source, data,sheetId,sheetKeyField);
        }
        
        public static bool UpdateSheetValue(this object source, ISpreadsheetData data)
        {
            return DefaultProcessor.UpdateSheetValue(source, data);
        }
        
        public static bool UpdateSheetValue(this object source, ISpreadsheetData data, string sheetId)
        {
            return DefaultProcessor.UpdateSheetValue(source, data,sheetId);
        }

        public static List<Object> SyncFolderAssets(
            this Type filterType, 
            string folder,
            ISpreadsheetData spreadsheetData,
            Object[] assets = null,
            bool createMissing = true, 
            int maxItemsCount = -1,
            string overrideSheetId = "")
        {
            return DefaultProcessor.SyncFolderAssets(filterType, folder, spreadsheetData,assets, createMissing, maxItemsCount, overrideSheetId);
        }
        
        public static List<Object> SyncFolderAssets(
            this Type type, 
            string folder,
            bool createMissing, 
            ISpreadsheetData spreadsheetData)
        {
            return DefaultProcessor.SyncFolderAssets(type, folder, createMissing, spreadsheetData);
        }
        
        public static object ApplySpreadsheetData(
            this object asset,
            ISpreadsheetData spreadsheetData, 
            string sheetName,
            object keyValue = null,
            string sheetFieldName = "")
        {
            if (spreadsheetData.HasSheet(sheetName) == false)
                return asset;
            
            var syncAsset = asset.CreateSheetScheme();

            var sheetValueIndo = new SheetValueInfo
            {
                Source = asset,
                SheetName = sheetName,
                SpreadsheetData = spreadsheetData,
                SyncScheme = syncAsset,
                SyncFieldName = sheetFieldName,
                SyncFieldValue = keyValue,
                StartColumn = 0
            };
            
            return DefaultProcessor.ApplyData(sheetValueIndo);
        }

        public static object ApplySpreadsheetData(
            this object asset, 
            SheetValueInfo sheetValueInfo, 
            string sheetName,
            object keyValue = null,
            string sheetFieldName = "",
            int overrideStartColumn = 0)
        {
            if (!sheetValueInfo.SpreadsheetData.HasSheet(sheetName))
                return asset;
            
            sheetValueInfo.SheetName = sheetName;
            sheetValueInfo.SyncFieldName = sheetFieldName;
            sheetValueInfo.SyncFieldValue = keyValue;
            sheetValueInfo.StartColumn = overrideStartColumn;

            return DefaultProcessor.ApplyData(sheetValueInfo);
        }

        public static object ApplySpreadsheetData(this object asset, ISpreadsheetData data)
        {
            return DefaultProcessor.ApplyData(asset,data);
        }
        
        public static object ApplySpreadsheetData(this object asset, SheetSyncScheme syncAsset, ISpreadsheetData data)
        {
            var sheetValueInfo = new SheetValueInfo
            {
                Source = asset,
                SyncScheme = syncAsset,
                SpreadsheetData = data,
                StartColumn = 0
            };
            return DefaultProcessor.ApplyData(sheetValueInfo);
        }

        public static object ApplySpreadsheetData(
            this object asset,
            Type type,
            ISpreadsheetData sheetData,
            string sheetId,
            object keyValue = null,
            string sheetFieldName = "")
        {
            var syncAsset = type.CreateSheetScheme();
            
            var sheetValue = new SheetValueInfo
            {
                Source = asset,
                SheetName = sheetId,
                SpreadsheetData = sheetData,
                SyncScheme = syncAsset,
                SyncFieldName = sheetFieldName,
                SyncFieldValue = keyValue,
                StartColumn = 0
            };
            
            return DefaultProcessor.ApplyData(sheetValue);
        }

        public static object ConvertType(this object source, Type target)
        {
            if (source == null)
                return null;
            
            if (target.IsInstanceOfType(source))
                return source;

            return ObjectTypeConverter.TypeConverters.ConvertValue(source, target);
        }

        public static IDictionary<T, V> GetDictionary<T, V>(this SheetData sheet, string keyColumn, string valueColumn)
        {
            var dict = new Dictionary<T, V>();
            
            foreach (var row in sheet.Rows.Cast<DataRow>())
            {
                var key = (T) Convert.ChangeType(row[keyColumn], typeof(T));
                var val = (V) Convert.ChangeType(row[valueColumn], typeof(V));
                dict.Add(key, val);
            }

            return dict;
        }
    }
}