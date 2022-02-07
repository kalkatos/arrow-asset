using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MKStudio.EditorLocalization
{
    // [CreateAssetMenu(fileName = "MKLanguage", menuName = "Scriptable Objects/MKLanguageSource", order = 1)]
    public class MKLanguageSource : ScriptableObject
    {    
        public int CurrentLanguageId { get; set; }


        public List<MKLocalizationItem> LocalizationList;

        private Dictionary<string, MKLocalizationItem> localizationDictionary;

        public void Initialize()
        {
            localizationDictionary = new Dictionary<string, MKLocalizationItem>();
            for (int i = 0; i < LocalizationList.Count; i++)
            {
                if (string.IsNullOrEmpty(LocalizationList[i].Key))
                {
                    Debug.LogError("Found empty key in the list.");
                    break;
                }
                if (localizationDictionary.ContainsKey(LocalizationList[i].Key))
                {
                    Debug.LogError("Found duplicate key in the list");
                    break;
                }
                localizationDictionary.Add(LocalizationList[i].Key, LocalizationList[i]);
            }
        }

        public string GetTranslation(string key)
        {
            if (localizationDictionary.ContainsKey(key))
            {
                return localizationDictionary[key].Translations[CurrentLanguageId];
            }
            else
            {
                Debug.LogWarning("Cannot find key " + key);
                return "Key Missing";
            }
        }
    }
}
