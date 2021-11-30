using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BennyKok.ReactiveProperty
{
    [System.Serializable]
    public class BoolProperty : TextBaseProperty<bool>
    {
        public BoolEvent valueChanged;
        public UnityEvent onTrue;
        public UnityEvent onFalse;
        
        public string yes;
        public string no;

        public override bool Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                valueChanged.Invoke(base.Value);
                
                if (base.Value)
                    onTrue.Invoke();
                else
                    onFalse.Invoke();
            }
        }

        public override bool Load(string key, bool defaultValue)
        {
            return PlayerPrefs.GetInt(key, (defaultValue ? 1 : 0)) != 0;
        }

        public override void Save(string key, bool value)
        {
            PlayerPrefs.SetInt(key, (value ? 1 : 0));
        }

        public override string GetTextForDisplay()
        {
            return Value ? yes : no;
        }
        
        public override string GetPersistenceValueDisplay()
        {
            return ( PlayerPrefs.GetInt(key) != 0 ? "True" : "False" );
        }
    }

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool>
    {
    }
}