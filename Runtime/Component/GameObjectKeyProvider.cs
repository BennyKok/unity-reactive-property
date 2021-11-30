#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif
using System;
using UnityEngine;

namespace BennyKok.ReactiveProperty
{
    public class GameObjectKeyProvider : MonoBehaviour, IPersistenceKeyProvider
    {
        public Component target;
        public bool autoName = true;
        public string customKeyPrefix;

        public string GetPersistenceKey(string currentKey)
        {
            var prefix = autoName ? gameObject.name : customKeyPrefix;
            return prefix + "." + currentKey;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GameObjectKeyProvider))]
    public class GameObjectKeyProviderEditor : Editor
    {
        private List<Property> savedProperties = new List<Property>();

        private SerializedProperty autoName;
        private SerializedProperty componentTarget;
        private SerializedProperty customKeyPrefix;

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            serializedObject.Update();

            var provider = target as GameObjectKeyProvider;

            EditorGUILayout.PropertyField(autoName);
            if (!autoName.boolValue)
                EditorGUILayout.PropertyField(customKeyPrefix);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(componentTarget);
            if (EditorGUI.EndChangeCheck())
            {
                Refresh();
            }

            EditorGUILayout.Space();

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("Saved Properties", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                {
                    for (int i = 0; i < savedProperties.Count; i++)
                    {
                        var property = savedProperties[i];
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            var persistenceKey = property.GetPersistenceKey(provider);
                            GUILayout.Label(persistenceKey);
                            GUILayout.FlexibleSpace();
                            GUILayout.Label(property.GetPersistenceValueDisplay(persistenceKey));
                            if (GUILayout.Button("Clear"))
                                PlayerPrefs.DeleteKey(persistenceKey);
                        }
                    }
                }
                EditorGUI.indentLevel--;
            }
            if (GUILayout.Button("Clear All"))
            {
                for (int i = 0; i < savedProperties.Count; i++)
                {
                    var property = savedProperties[i];
                    var persistenceKey = property.GetPersistenceKey(provider);
                    property.ClearPersistenceValue(persistenceKey);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            autoName = serializedObject.FindProperty("autoName");
            componentTarget = serializedObject.FindProperty("target");
            customKeyPrefix = serializedObject.FindProperty("customKeyPrefix");

            Refresh();
        }

        public void Refresh()
        {
            savedProperties.Clear();

            var refObject = componentTarget.objectReferenceValue;

            if (refObject == null)
                return;

            var allFields = refObject.GetType().GetFields();
            foreach (var field in allFields)
            {
                if (field.GetValue(refObject) is Property reflect && !string.IsNullOrEmpty(reflect.key))
                    savedProperties.Add(reflect);
            }
        }
    }
#endif
}