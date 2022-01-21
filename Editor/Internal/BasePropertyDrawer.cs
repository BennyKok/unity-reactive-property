using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BennyKok.ReactiveProperty.Editor
{
    public class BasePropertyDrawer : PropertyDrawer
    {
        public static int tabBtnWidth = 25;

        public static int verticalSpace = 2;
        public static int groupHorizontalSpace = 4;

        public static float subTitleVerticalSpace;

        SerializedProperty previousProp;

        public static GUIStyle miniButton;
        public static GUIStyle tabButton;
        public static GUIStyle activeTabButton;

        public class PropertyTab
        {
            public AnimBool visible;
            public GUIContent icon;
            public List<ItemContent> contents;
        }

        // public List<PropertyTab> allTabs = new List<PropertyTab>();

        //Have to do this because in array default editor, same PropertyDrawer will be used for multiple items
        public Dictionary<string, TabsState> states = new Dictionary<string, TabsState>();

        public class TabsState
        {
            public List<PropertyTab> tabs;
            public int editItem = -1;
            public int trackedEditItem;
        }

        public bool init;
        private TabsState currentState;

        public enum ItemType
        {
            Property,
            Header,
            HeaderButton,
            GUI
        }

        public class ItemContent
        {
            public SerializedProperty property;
            public ItemType type;
            public string name;
            public string buttonName;
            public Action buttonCallback;
            public SerializedProperty enableIf;
            public Func<bool> enableCallback;
            public Func<float> guiHeightCallback;
            public Action<Rect> guiDrawCallback;

            public ItemContent(ItemType type)
            {
                this.type = type;
            }

            public ItemContent(SerializedProperty property, ItemType type, string name)
            {
                this.type = type;
                this.name = name;

                if (type == ItemType.Property)
                {
                    this.property = property.FindPropertyRelative(name);
                }
            }

            public ItemContent(SerializedProperty property, ItemType type, string name, string buttonName,
                Action buttonCallback)
                : this(property, type, name)
            {
                this.buttonName = buttonName;
                this.buttonCallback = buttonCallback;
            }
        }

        public float GetItemsHeight(List<ItemContent> contents)
        {
            return contents.Sum(
                (x) =>
                {
                    if (x.enableIf != null && !x.enableIf.boolValue)
                    {
                        return 0;
                    }

                    if (x.enableCallback != null && !x.enableCallback.Invoke())
                    {
                        return 0;
                    }

                    if (x.guiHeightCallback != null)
                    {
                        return x.guiHeightCallback.Invoke();
                    }

                    if (x.type == ItemType.Property)
                    {
                        return EditorGUI.GetPropertyHeight(x.property) + verticalSpace;
                    }
                    else
                    {
                        return subTitleVerticalSpace + verticalSpace;
                    }
                }
            ) + verticalSpace * 2;
        }

        public void Init(SerializedProperty property)
        {
            if (!init)
            {
                init = true;

                miniButton = new GUIStyle("button");
                miniButton.fontSize = 10;

                tabButton = new GUIStyle(EditorStyles.toolbarButton);
                tabButton.fixedHeight = EditorGUIUtility.singleLineHeight;

                activeTabButton = new GUIStyle(tabButton);
                activeTabButton.normal.background = EditorStyles.miniButton.normal.background;

                previousProp = property;

                Refresh(property);
            }
            else if (property.propertyPath != previousProp.propertyPath)
            {
                Refresh(property);
            }
        }

        public List<PropertyTab> GetCurrentTab(SerializedProperty property)
        {
            return GetCurrentState(property).tabs;
        }

        public TabsState GetCurrentState(SerializedProperty property)
        {
            TabsState currentTabState;
            if (!states.TryGetValue(property.propertyPath, out currentTabState))
            {
                currentTabState = new TabsState();
                currentTabState.tabs = new List<PropertyTab>();
                states.Add(property.propertyPath, currentTabState);
            }

            return currentTabState;
        }

        public void Refresh(SerializedProperty property)
        {
            var isNew = false;
            var state = GetCurrentState(property);
            var currentTab = state.tabs;
            isNew = currentTab.Count == 0;

            if (isNew)
            {
                InitTab(property, currentTab);
                foreach (var tab in currentTab)
                {
                    tab.visible = new AnimBool();
                    tab.visible.speed = DrawerUtil.AnimSpeed;
                    tab.visible.valueChanged.AddListener(() =>
                    {
                        DrawerUtil.RepaintInspector(property.serializedObject);
                    });
                }

                var openedTab = property.FindPropertyRelative("editor_openedTab");

                if (property.isExpanded && openedTab.intValue < currentTab.Count)
                {
                    currentTab[openedTab.intValue].visible.value = true;
                    state.editItem = openedTab.intValue;
                    state.trackedEditItem = openedTab.intValue;
                }
            }
        }

        public virtual void InitTab(SerializedProperty property, List<PropertyTab> tabs)
        {
            tabs.Clear();
        }

        public void CloseAllTab(SerializedProperty property)
        {
            foreach (var tab in GetCurrentTab(property))
            {
                tab.visible.target = false;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);

            var allTabs = GetCurrentTab(property);

            var valueProperty = property.FindPropertyRelative("value");
            var persistenceProperty = property.FindPropertyRelative("persistence");

            if (persistenceProperty.boolValue)
                label.text += " (Saved)";

            EditorGUI.BeginProperty(position, label, property);

            var propertyRect = new Rect(position.x, position.y, position.width - tabBtnWidth * allTabs.Count,
                EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propertyRect, valueProperty, label);

            currentState = GetCurrentState(property);
            for (int i = 0; i < allTabs.Count; i++)
            {
                var tab = allTabs[i];
                var btnRect = new Rect(position.x + position.width - (allTabs.Count - i) * tabBtnWidth, position.y,
                    tabBtnWidth, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(btnRect, tab.icon, currentState.editItem == i ? activeTabButton : tabButton))
                {
                    if (currentState.editItem == i)
                    {
                        currentState.editItem = -1;
                        CloseAllTab(property);

                        property.isExpanded = false;
                    }
                    else
                    {
                        currentState.trackedEditItem = i;
                        currentState.editItem = i;
                        CloseAllTab(property);
                        tab.visible.target = true;

                        property.isExpanded = true;
                        property.FindPropertyRelative("editor_openedTab").intValue = i;
                    }
                }
            }

            subTitleVerticalSpace = EditorStyles.miniBoldLabel.CalcHeight(GUIContent.none, position.width);

            var extraRectGroup = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + verticalSpace,
                position.width, EditorGUIUtility.singleLineHeight);
            extraRectGroup = EditorGUI.IndentedRect(extraRectGroup);

            var displayTab = allTabs[currentState.trackedEditItem];
            if (DrawerUtil.BeginFade(displayTab.visible))
            {
                extraRectGroup.height = GetItemsHeight(displayTab.contents);

                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                GUI.BeginGroup(extraRectGroup, EditorStyles.helpBox);

                DrawItemContents(displayTab.contents, extraRectGroup.width);

                GUI.EndGroup();

                EditorGUI.indentLevel = indent;
            }

            DrawerUtil.EndFade();

            EditorGUI.EndProperty();
        }

        public void DrawItemContents(List<ItemContent> contents, float width)
        {
            var indent = 12;
            var extraRect = new Rect(groupHorizontalSpace + indent, verticalSpace,
                width - groupHorizontalSpace * 2 - indent,
                EditorGUIUtility.singleLineHeight);
            foreach (var item in contents)
            {
                if (item.enableIf != null && !item.enableIf.boolValue)
                {
                    continue;
                }

                if (item.enableCallback != null && !item.enableCallback.Invoke())
                {
                    continue;
                }

                switch (item.type)
                {
                    case ItemType.Property:
                        EditorGUI.PropertyField(extraRect, item.property);
                        extraRect.y += EditorGUI.GetPropertyHeight(item.property) + verticalSpace;
                        break;
                    case ItemType.Header:
                        EditorGUI.LabelField(extraRect, item.name, EditorStyles.miniBoldLabel);
                        extraRect.y += subTitleVerticalSpace + verticalSpace;
                        break;
                    case ItemType.HeaderButton:
                        EditorGUI.LabelField(extraRect, item.name, EditorStyles.miniBoldLabel);

                        var buttonRect = new Rect(width - 50 - groupHorizontalSpace, extraRect.y, 50, 16);
                        if (GUI.Button(buttonRect, item.buttonName, miniButton))
                        {
                            item.buttonCallback.Invoke();
                        }

                        extraRect.y += subTitleVerticalSpace + verticalSpace;
                        break;
                    case ItemType.GUI:
                        item.guiDrawCallback.Invoke(extraRect);
                        extraRect.y += item.guiHeightCallback.Invoke() + verticalSpace;
                        break;
                }
            }
        }

        public bool isEditingScriptableObject(SerializedProperty property)
        {
            return property.serializedObject.targetObject is ScriptableObject;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Init(property);

            var extraHeight = 0f;

            var allTabs = GetCurrentTab(property);

            var displayTab = allTabs[GetCurrentState(property).trackedEditItem];
            extraHeight += (GetItemsHeight(displayTab.contents) + 6) * displayTab.visible.faded;

            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value")) + extraHeight;
        }
    }
}