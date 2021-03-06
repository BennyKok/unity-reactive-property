using UnityEngine.Events;
using UnityEngine.UI;

namespace BennyKok.ReactiveProperty
{
    public class ButtonProperty : BaseProperty<UnityAction>
    {
        public ButtonProperty()
        {
        }

        public ButtonProperty(UnityAction action)
        {
            Value = action;
        }

        [System.NonSerialized]
        public UnityEngine.UI.Button targetButton;

        public override void OnCreateResolver()
        {
            AddResolver<UnityEngine.UI.Button>((button, state) =>
            {
                switch (state)
                {
                    case ResolveSate.Bind:
                        targetButton = button;
                        button.onClick.AddListener(Value);
                        break;
                    case ResolveSate.UnBind:
                        targetButton = null;
                        button.onClick.RemoveListener(Value);
                        break;
                }
            });
        }
    }

}