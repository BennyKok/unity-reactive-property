using System;
using UnityEngine;

namespace BennyKok.ReactiveProperty
{
    public abstract class Property
    {
#if UNITY_EDITOR
        public int editor_openedTab = 0;
#endif
        
        protected IPersistenceKeyProvider keyProvider;
        public virtual void InitValue(IPersistenceKeyProvider keyProvider = null) => this.keyProvider = keyProvider;
        public abstract bool CanBind();

        public string key;

        public string GetPersistenceKey(IPersistenceKeyProvider customKeyProvider = null)
        {
            if (customKeyProvider != null) return customKeyProvider.GetPersistenceKey(key);
            if (keyProvider == null) return key;
            return keyProvider.GetPersistenceKey(key);
        }

        public virtual bool BindListener(GameObject target, bool emitEvent = false)
        {
            return false;
        }

        public virtual void UnBindListener(GameObject target, bool emitEvent = false)
        {
            keyProvider = null;
        }

        public virtual string GetPersistenceValueDisplay(string key)
        {
            return null;
        }
        
        public virtual void ClearPersistenceValue(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}