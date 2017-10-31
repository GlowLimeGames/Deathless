using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(WorldItem))]
public class WorldItemPropertyDrawer : Editor {
    SerializedProperty interactable;

    private void OnEnable() {
        interactable = serializedObject.FindProperty("interactable");
    }        

    public override void OnInspectorGUI() {
        serializedObject.Update();
        interactable.boolValue = EditorGUILayout.Toggle("Interactable", interactable.boolValue);
        serializedObject.ApplyModifiedProperties();
        if (GUILayout.Button("Calculate Z-Pos")) {
           ( (WorldItem) target).UpdateZPos(false);
        }
        if (interactable.boolValue) {
            DrawDefaultInspector();
        }
    }
}