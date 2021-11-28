using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace BennyKok.ReactiveProperty.Editor
{
    [CustomPropertyDrawer(typeof(BoolProperty))]
    public class BoolPropertyDrawer : TextBasePropertyDrawer
    {
        public override void InitTab(SerializedProperty property, List<PropertyTab> tabs)
        {
            base.InitTab(property, tabs);

            tabs[0].contents.Add(new ItemContent(property, ItemType.Property, "yes"));
            tabs[0].contents.Add(new ItemContent(property, ItemType.Property, "no"));
            
            tabs[1].contents.Add(new ItemContent(property, ItemType.Property, "onTrue"));
            tabs[1].contents.Add(new ItemContent(property, ItemType.Property, "onFalse"));

            tabs[2].contents.Add(
                new ItemContent(ItemType.GUI)
                {
                    enableIf = property.FindPropertyRelative("persistence"),
                    guiHeightCallback = () => EditorGUIUtility.singleLineHeight + verticalSpace,
                    guiDrawCallback = (rect) =>
                    {
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUI.TextField(rect, PlayerPrefs.GetInt(property.FindPropertyRelative("key").stringValue) != 0 ? "True" : "False");
                        EditorGUI.EndDisabledGroup();
                    }
                }
            );
        }
    }

}