using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BennyKok.ReactiveProperty
{
    [System.Serializable]
    public class Vector3Property : TextBaseProperty<Vector3>
    {
        public Vector3Event valueChanged;

        public override Vector3 Value
        {
            get { return base.Value; }
            set
            {
                var oldValue = base.Value;
                base.Value = value;
                valueChanged.Invoke(base.Value);
            }
        }

        public override Vector3 Load(string key, Vector3 defaultValue)
        {
            var result = new Vector3(
                PlayerPrefs.GetFloat(key + ".x", defaultValue.x),
                PlayerPrefs.GetFloat(key + ".y", defaultValue.y),
                PlayerPrefs.GetFloat(key + ".z", defaultValue.z)
            );
            return result;
        }

        public override void Save(string key, Vector3 value)
        {
            PlayerPrefs.SetFloat(key + ".x", value.x);
            PlayerPrefs.SetFloat(key + ".y", value.y);
            PlayerPrefs.SetFloat(key + ".z", value.z);
        }
        
        public override void ClearPersistenceValue(string key)
        {
            PlayerPrefs.DeleteKey(key + ".x");
            PlayerPrefs.DeleteKey(key + ".y");
            PlayerPrefs.DeleteKey(key + ".z");
        }
    }

    [System.Serializable]
    public class Vector3Event : UnityEvent<Vector3>
    {
    }
}