#if UNITY_EDITOR

using System.Collections.Generic;
using Language.Editor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace KatAudio.Editor
{
    [CustomPropertyDrawer(typeof(SoundSearchField))]
    public class SoundSearchFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String) return;
            
            Rect labelPos = new Rect(position.x, position.y, 200, 16f);
            Rect textField = new Rect(position.x + labelPos.width, position.y, position.width - labelPos.width, 16f);
            Rect buttonPos = new Rect(position.x, position.y + 18f, position.width, 17f);
            EditorGUI.LabelField(labelPos, label);
            if (!property.serializedObject.isEditingMultipleObjects)
            {
                property.stringValue = EditorGUI.TextField(textField, property.stringValue);
                property.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                EditorGUI.LabelField(textField, property.stringValue);
            }
            if (GUI.Button(buttonPos, "Search"))
            {
                StringSearch search = ScriptableObject.CreateInstance<StringSearch>();
                var guids = AssetDatabase.FindAssets("t:AudioClip", new[] { (attribute as SoundSearchField).Path  });
                
                search.Keys = new List<string>();
                foreach (var guid in guids)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    AudioClip data = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

                    if (data)
                    {
                        search.Keys.Add(data.name);
                    }
                }
                
                search.Callback = (value) =>
                {
                    property.stringValue = value;
                    property.serializedObject.ApplyModifiedProperties();
                };
                SearchWindow.Open(new SearchWindowContext(GUIUtility
                    .GUIToScreenPoint(Event.current.mousePosition)), search);   
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 34f;
        }
    }
}
#endif