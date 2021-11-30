using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace BennyKok.ReactiveProperty
{
    [System.Serializable]
    public abstract class PersistenceProperty<T> : BaseProperty<T>
    {
        [FormerlySerializedAs("persistance")] public bool persistence;

        public string key;

        [System.NonSerialized] public bool loaded;

        public override T Value
        {
            get
            {
                if (persistence && !loaded)
                {
                    var saved = Load(GetPersistenceKey(key), base.Value);
                    loaded = true;
                    UpdateValueInternal(saved);
                    return saved;
                }

                return base.Value;
            }
            set
            {
                base.Value = value;
                if (persistence)
                {
                    Save(GetPersistenceKey(key), value);
                }
            }
        }

        public string GetPersistenceKey(string key)
        {
            if (keyProvider == null) return key;
            return keyProvider.GetPersistenceKey(key);
        }

        public abstract void Save(string key, T value);

        public abstract T Load(string key, T defaultValue);
    }
}