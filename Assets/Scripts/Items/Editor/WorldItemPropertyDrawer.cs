using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(WorldItem))]
public class WorldItemPropertyDrawer : Editor {
    SerializedProperty interactable;
    SerializedProperty animate;
    //possibly create the serializedproperty of zpos calculation here 
    private void OnEnable() {
        interactable = serializedObject.FindProperty("interactable");
        animate = serializedObject.FindProperty("isAnimate");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        interactable.boolValue = EditorGUILayout.Toggle("Interactable", interactable.boolValue);
        animate.boolValue = EditorGUILayout.Toggle("IsAnimate", animate.boolValue);
        serializedObject.ApplyModifiedProperties();

        if (interactable.boolValue) {
            DrawDefaultInspector();
        }

    }
}