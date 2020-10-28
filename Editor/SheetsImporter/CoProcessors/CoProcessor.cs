using System;
using System.Collections.Generic;
using System.Linq;
using UniModules.UniGame.Core.EditorTools.Editor;
using UniModules.UniGame.Core.EditorTools.Editor.AssetOperations;
using UniModules.UniGame.Core.EditorTools.Editor.Tools;
using UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.CoProcessors.Abstract;
using UnityEngine;

namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.CoProcessors
{
    [CreateAssetMenu(menuName = "UniGame/Google/CoProcessors/CoProcessor", fileName = nameof(CoProcessor))]
    public class CoProcessor : ScriptableObject, ICoProcessorHandle
    {
        #region static data

        private static string _defaultCoProcessorPath;

        private static string DefaultCoProcessorPath => _defaultCoProcessorPath =
            string.IsNullOrEmpty(_defaultCoProcessorPath)
                ? EditorFileUtils.Combine(EditorPathConstants.GeneratedContentPath,
                    "GoogleSheetImporter/Editor/CoProcessors/")
                : _defaultCoProcessorPath;

        private static CoProcessor _processor;

        public static CoProcessor Processor
        {
            get
            {
                if (_processor)
                    return _processor;

                _processor = AssetEditorTools.GetAsset<CoProcessor>();
                if (!_processor)
                {
                    _processor = CreateInstance<CoProcessor>();
                    _processor.SaveAsset(nameof(CoProcessor), DefaultCoProcessorPath);
                }

                return _processor;
            }
        }

        #endregion
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor(Sirenix.OdinInspector.InlineEditorModes.GUIOnly,
            Sirenix.OdinInspector.InlineEditorObjectFieldModes.Foldout)]
#endif
        [SerializeField]
        private List<BaseCoProcessor> _processors = new List<BaseCoProcessor>();
        
        public bool CanApply(string columnName)
        {
            if(string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException(nameof(columnName));
            
            return _processors.Any(x => x.CanApply(columnName));
        }

        public void Apply(string columnName)
        {
            if(string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException(nameof(columnName));

            var validProcessor = _processors.FirstOrDefault(x => x.CanApply(columnName));
            if (validProcessor != null)
            {
                validProcessor.Apply(columnName);
            }
        }
    }
}