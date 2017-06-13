using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dialogue {
    [UnityEditor.CustomEditor(typeof(NodeData))]
    public class NodeEditor : Editor {
        public static bool Link { get; set; }
        private SerializedObject dataObject;
        private NodeData lastTarget;

        public override void OnInspectorGUI() {
            NodeData data = (NodeData)target;
            if (dataObject != null) { dataObject.ApplyModifiedProperties(); }

            if (data != lastTarget) {
                dataObject = new SerializedObject(data);
                lastTarget = data;
            }

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