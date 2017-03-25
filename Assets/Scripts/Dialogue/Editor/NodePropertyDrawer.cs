using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dialogue {
    // currently unused
    [UnityEditor.CustomPropertyDrawer(typeof(NodeData))]
    public class NodePropertyDrawer : UnityEditor.PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            
            SerializedObject data = new SerializedObject(property.objectReferenceValue);

            SerializedProperty text = data.FindProperty("text");
            SerializedProperty speaker = data.FindProperty("speaker");
            SerializedProperty notes = data.FindProperty("notes");
            SerializedProperty condition = data.FindProperty("condition");
            SerializedProperty action = data.FindProperty("action");

            if (text != null) { EditorGUILayout.PropertyField(text); }
            if (speaker != null) { EditorGUILayout.PropertyField(speaker); }
            if (notes != null) { EditorGUILayout.PropertyField(notes); }
            if (condition != null) { EditorGUILayout.PropertyField(condition); }
            if (action != null) { EditorGUILayout.PropertyField(action); }
            
            data.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

    }

    [UnityEditor.CustomEditor(typeof(NodeData))]
    public class NodeEditor : Editor {
        public static bool Link { get; set; }

        public override void OnInspectorGUI() {
            NodeData data = (NodeData)target;
            GUIStyle style;

            EditorGUI.BeginDisabledGroup(Link);
            
            EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);
            style = new GUIStyle(EditorStyles.textArea);
            style.wordWrap = true;
            data.Text = EditorGUILayout.TextArea(data.Text, style);

            EditorGUI.EndDisabledGroup();
        }
    }
}