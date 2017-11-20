using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dialogue {
    [UnityEditor.CustomEditor(typeof(NodeData))]
    public class NodeEditor : Editor {
        public static bool Editable { get; set; }
        private SerializedObject dataObject;
        private NodeData lastTarget;
        private bool showOnce;

        public override void OnInspectorGUI() {
            NodeData data = (NodeData)target;
            if (dataObject != null) { dataObject.ApplyModifiedProperties(); }

            if (data != lastTarget) {
                dataObject = new SerializedObject(data);
                lastTarget = data;
            }

            GUIStyle style;

            EditorGUI.BeginDisabledGroup(!Editable);
            
            EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);
            style = new GUIStyle(EditorStyles.textArea);
            style.wordWrap = true;
            data.Text = EditorGUILayout.TextArea(data.Text, style);

            EditorGUILayout.LabelField("Notes", EditorStyles.boldLabel);
            style = new GUIStyle(EditorStyles.textArea);
            style.wordWrap = true;
            data.Notes = EditorGUILayout.TextArea(data.Notes, style);

            SerializedProperty speaker = dataObject.FindProperty("speaker");
            EditorGUILayout.PropertyField(speaker);

            SerializedProperty restriction = dataObject.FindProperty("restriction");
            EditorGUILayout.PropertyField(restriction);
            
            data.EndDialogue = EditorGUILayout.Toggle("End Dialogue", data.EndDialogue);
            data.RandomizeChildren = EditorGUILayout.Toggle("Randomize Children", data.RandomizeChildren);

            EditorGUI.EndDisabledGroup();
        }
    }
}