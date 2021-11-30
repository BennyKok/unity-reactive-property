using System;
using UnityEngine;

namespace BennyKok.ReactiveProperty
{
    public abstract class Property
    {
        protected IPersistenceKeyProvider keyProvider;
        public virtual void InitValue(IPersistenceKeyProvider keyProvider = null) => this.keyProvider = keyProvider;
        public abstract bool CanBind();

        public virtual bool BindListener(GameObject target, bool emitEvent = false)
        {
            return false;
        }

        public virtual void UnBindListener(GameObject target, bool emitEvent = false)
        {
            keyProvider = null;
        }
    }
}