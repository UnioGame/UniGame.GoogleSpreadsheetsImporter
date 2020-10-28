using System;
using UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.CoProcessors.Abstract;
using UnityEngine;

namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.SheetsImporter.CoProcessors
{
    [Serializable]
    public abstract class BaseCoProcessor : ScriptableObject, ICoProcessorHandle
    {
        public abstract bool CanApply(string columnName);
        public abstract void Apply(string columnName);
    }
}