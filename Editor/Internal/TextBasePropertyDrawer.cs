using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using System;
using System.Reflection;

namespace BennyKok.ReactiveProperty.Editor
{
    public class TextBasePropertyDrawer : BasePropertyDrawer
    {
        public override void InitTab(SerializedProperty property, List<PropertyTab> tabs)
        {
            base.InitTab(property, tabs);

            var tab1 = new PropertyTab();
            tab1.icon = EditorGUIUtility.IconContent("Add-Available","UI Bindings");
            tab1.contents = new List<ItemContent>()
            {
                new ItemContent(property, ItemType.HeaderButton, "UI Binding", "Find", () =>
                {
                    var attempt = GameObject.Find(property.displayName);
#if HAS_TMP
                    if (attempt && attempt.TryGetComponent<TMPro.TextMeshProUGUI>(out var temp))
                    {
                        tab1.contents[1].property.objectReferenceValue = attempt;
                        Debug.Log("Find matched UI for (" + property.displayName + ")");
                    }
                    else
                    {
                        Debug.LogWarning("Couldn't find any matched UI for (" + property.displayName + ")");
                    }
#endif
                })
                {
                    enableCallback = () => !isEditingScriptableObject(property)
                },
                new ItemContent(property, ItemType.Property, "target")
                {
                    enableCallback = () => !isEditingScriptableObject(property)
                },

                new ItemContent(property, ItemType.Header, "Extra"),
                new ItemContent(property, ItemType.Property, "prefix"),
                new ItemContent(property, ItemType.Property, "suffix")
            };
            tabs.Add(tab1);

            tabs.Add(new PropertyTab()
            {
                icon = EditorGUIUtility.IconContent("EventSystem Icon", "Events"),
                contents = new List<ItemContent>()
                {
                    new ItemContent(property, ItemType.Header, "Event"),
                    new ItemContent(property, ItemType.Property, "valueChanged")
                }
            });
            
            var tab3 = new PropertyTab();
            tab3.icon = EditorGUIUtility.IconContent("_Popup","Persistence");
            tab3.contents = new List<ItemContent>()
            {
                new ItemContent(property, ItemType.HeaderButton, "Persistence", "Auto", () =>
                {
                    tab3.contents[1].property.boolValue = true;

                    var name = property.serializedObject.targetObject.name;
                    tab3.contents[2].property.stringValue = ToCamelCase(name) + "." + property.propertyPath;
                    // ToCamelCase(name) + property.displayName.Replace(" ","");
                }),
                new ItemContent(property, ItemType.Property, "persistence"),
                new ItemContent(property, ItemType.Property, "key")
                {
                    enableIf = property.FindPropertyRelative("persistence"),
                },
                new ItemContent(property, ItemType.HeaderButton, "Player Prefs", "Reset",
                    () => { PlayerPrefs.DeleteKey(property.FindPropertyRelative("key").stringValue); })
                {
                    enableIf = property.FindPropertyRelative("persistence"),
                },
                new ItemContent(ItemType.GUI)
                {
                    enableIf = property.FindPropertyRelative("persistence"),
                    guiHeightCallback = () => EditorGUIUtility.singleLineHeight + verticalSpace,
                    guiDrawCallback = (rect) =>
                    {
                        var target = GetTargetObjectOfProperty(property) as Property;
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUI.TextField(rect, target?.GetPersistenceValueDisplay(target.GetPersistenceKey()));
                        EditorGUI.EndDisabledGroup();
                    }
                }
            };
            tabs.Add(tab3);
        }

        public static string ToCamelCase(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return Char.ToLowerInvariant(str[0]) + str.Substring(1).Replace(" ", "");
            }

            return str;
        }
        
        /// ref https://github.com/lordofduct/spacepuppy-unity-framework/blob/master/SpacepuppyBaseEditor/EditorHelper.cs
        /// <summary>
        /// Gets the object the property represents.
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static object GetTargetObjectOfProperty(SerializedProperty prop)
        {
            if (prop == null) return null;

            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue_Imp(obj, elementName, index);
                }
                else
                {
                    obj = GetValue_Imp(obj, element);
                }
            }
            return obj;
        }
        
        private static object GetValue_Imp(object source, string name)
        {
            if (source == null)
                return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null)
                    return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null)
                    return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        private static object GetValue_Imp(object source, string name, int index)
        {
            var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            //while (index-- >= 0)
            //    enm.MoveNext();
            //return enm.Current;

            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }

    }
}