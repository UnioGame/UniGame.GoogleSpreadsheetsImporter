namespace UniModules.UniGame.GoogleSpreadsheetsImporter.Editor.EditorWindow
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SheetCellView : ScriptableObject
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideLabel]
#endif
        public string value; 

        [HideInInspector]
        public int row;
        [HideInInspector]
        public int column;
        [HideInInspector]
        public bool isHeader;
    }
}