using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace BennyKok.ReactiveProperty.Editor
{
    [CustomPropertyDrawer(typeof(SpriteProperty))]
    public class SpritePropertyDrawer : BasePropertyDrawer
    {
        public override void InitTab(SerializedProperty property, List<PropertyTab> tabs)
        {
            base.InitTab(property, tabs);
            var tab1 = new PropertyTab();
            tab1.icon = EditorGUIUtility.IconContent("Font Icon");
            tab1.contents = new List<ItemContent>()
            {
                new ItemContent(property, ItemType.HeaderButton, "Binding","Find",()=>{
                    var attempt = GameObject.Find(property.displayName);
        
                    SpriteRenderer temp;
                    if (attempt && (temp = attempt.GetComponent<SpriteRenderer>()))
                    {
                        tab1.contents[1].property.objectReferenceValue = temp;
                    }
                    else
                    {
                        Debug.LogWarning("Couldn't find any matched SpriteRenderer for (" + property.displayName + ")");
                    }
                }){
                    enableCallback = ()=> !isEditingScriptableObject(property)
                },
                new ItemContent(property, ItemType.Property, "target"){
                    enableCallback = ()=> !isEditingScriptableObject(property)
                },
            };
            tabs.Add(tab1);

            tabs.Add(new PropertyTab()
            {
                icon = EditorGUIUtility.IconContent("EventSystem Icon"),
                contents = new List<ItemContent>()
                {
                    new ItemContent(property, ItemType.Header, "Event"),
                    new ItemContent(property, ItemType.Property, "valueChanged")
                }
            });
        }
    }

}