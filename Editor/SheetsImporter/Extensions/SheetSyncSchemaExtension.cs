namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter
{
    using System;
    using UniModules.UniCore.Runtime.Utils;

    public static class SheetSyncSchemaExtension
    {
        private static SheetSyncSchemaProcessor _syncSchemeProcessor = new SheetSyncSchemaProcessor();
        private static System.Func<Type, SheetSyncScheme> _syncCache =
            MemorizeTool.Create<Type, SheetSyncScheme>(x => _syncSchemeProcessor.CreateSyncScheme(x));
        public static SheetSyncScheme DummyScheme = new SheetSyncScheme(string.Empty);

        public static bool IsInIgnoreCache(this SheetValueInfo valueInfo)
        {
            return valueInfo != null &&
                   valueInfo.Source!=null &&
                   valueInfo.IgnoreCache != null &&
                   valueInfo.IgnoreCache.Contains(valueInfo.Source) == false;
        }
        
        public static bool IsInIgnoreCache(this SheetValueInfo valueInfo,object target)
        {
            return valueInfo != null &&
                   target != null &&
                   valueInfo.IgnoreCache != null &&
                   valueInfo.IgnoreCache.Contains(target) == false;
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