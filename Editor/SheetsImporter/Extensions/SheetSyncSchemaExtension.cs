namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.Extensions
{
    using System;
    using UniCore.Runtime.Utils;
    
    public static class SheetSyncSchemaExtension
    {
        private static readonly SheetSyncSchemaProcessor _syncSchemeProcessor = new SheetSyncSchemaProcessor();
        
        private static readonly Func<Type, SheetSyncScheme> _syncCache =
            MemorizeTool.Create<Type, SheetSyncScheme>(x => _syncSchemeProcessor.CreateSyncScheme(x));

        private static readonly SheetSyncScheme DummyScheme = new SheetSyncScheme(string.Empty);

        public static bool IsInIgnoreCache(this SheetValueInfo valueInfo)
        {
            return valueInfo != null &&
                   valueInfo.Source!=null &&
                   valueInfo.IgnoreCache != null &&
                   valueInfo.IgnoreCache.Contains(valueInfo.Source);
        }
        
        public static bool IsInIgnoreCache(this SheetValueInfo valueInfo, object target)
        {
            return valueInfo != null &&
                   target != null &&
                   valueInfo.IgnoreCache != null &&
                   valueInfo.IgnoreCache.Contains(target);
        }
        
        public static SheetSyncScheme CreateSheetScheme(this Type source)
        {
            if (source == null) {
                return DummyScheme;
            }

            return _syncCache(source);
        }
        
        public static SheetSyncScheme CreateSheetScheme(this object source)
        {
            if (source == null) {
                return DummyScheme;
            }

            var type = source.GetType();
            return CreateSheetScheme(type);
        }
    }
}