using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BennyKok.ReactiveProperty
{
    [System.Serializable]
    public abstract class TextBaseProperty<T> : PersistenceProperty<T>
    {
#if HAS_TMP
        public TMPro.TMP_Text targetText;
#endif
        public string prefix;
        public string suffix;

        public override T Value
        {
            get => base.Value;
            set => base.Value = value;
        }

        public override void OnCreateResolver()
        {
#if HAS_TMP
            AddResolver<TMPro.TMP_Text>((label, state) =>
            {
                switch (state)
                {
                    case ResolveSate.Update:
                        targetText = label;
                        label.text = GetFinalTextDisplay();
                        break;
                    case ResolveSate.UnBind:
                        targetText = null;
                        label.text = null;
                        break;
                }
            });
#endif
        }

        public string GetFinalTextDisplay()
        {
            return prefix + GetTextForDisplay() + suffix;
        }

        public virtual string GetTextForDisplay()
        {
            return Value.ToString();
        }
    }
}