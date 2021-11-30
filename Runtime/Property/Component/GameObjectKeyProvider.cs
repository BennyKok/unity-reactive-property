using UnityEngine;

namespace BennyKok.ReactiveProperty
{
    public class GameObjectKeyProvider : MonoBehaviour, IPersistenceKeyProvider
    {
        public bool autoName;
        public string customKeyPrefix;

        public string GetPersistenceKey(string currentKey)
        {
            var prefix = autoName ? gameObject.name : customKeyPrefix;
            return prefix + "." + currentKey;
        }
    }
}