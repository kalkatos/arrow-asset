using System;
using UnityEngine;


namespace MKStudio.EditorLocalization
{
    [Serializable]
    public class MKLocalizationItem
    {
        public string Key;
        [TextArea]
        public string[] Translations;
    }
}
