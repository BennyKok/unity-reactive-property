using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace BennyKok.ReactiveProperty.Editor
{
    [CustomPropertyDrawer(typeof(FloatProperty))]
    public class FloatPropertyDrawer : TextBasePropertyDrawer
    {
        public override void InitTab(SerializedProperty property, List<PropertyTab> tabs)
        {
            base.InitTab(property, tabs);

            // tabs[0].contents.Insert(2,new ItemContent(property, ItemType.Property, "fillTarget"){
            //     enableCallback = ()=> !isEditingScriptableObject(property)
            // });

            tabs[0].contents.Add(new ItemContent(property, ItemType.Property, "roundDigit"));
            tabs[1].contents.Add(new ItemContent(property, ItemType.Property, "valueIncreased"));
            tabs[1].contents.Add(new ItemContent(property, ItemType.Property, "valueDecreased"));

            // tabs[2].contents.Add(
            //     new ItemContent(ItemType.GUI)
            //     {
            //         enableIf = property.FindPropertyRelative("persistence"),
            //         guiHeightCallback = () => EditorGUIUtility.singleLineHeight + verticalSpace,
            //         guiDrawCallback = (rect) =>
            //         {
            //             EditorGUI.BeginDisabledGroup(true);
            //             EditorGUI.TextField(rect, PlayerPrefs.GetFloat(property.FindPropertyRelative("key").stringValue).ToString());
            //             EditorGUI.EndDisabledGroup();
            //         }
            //     }
            // );
        }
    }

}