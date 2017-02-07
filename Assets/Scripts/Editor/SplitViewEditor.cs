using UnityEngine;
using UnityEditor;

public class SplitViewWindow : EditorWindow {
    protected float currentScrollViewHeight;
    private Vector2 scrollPos = Vector2.zero;
    bool resize = false;

    void OnEnable() {
        currentScrollViewHeight = this.position.height / 2;
    }

    void OnGUI() {
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(currentScrollViewHeight));
        for (int i = 0; i < 20; i++)
            GUILayout.Label("dfs");
        GUILayout.EndScrollView();

        ResizableSplit();

        GUILayout.FlexibleSpace();
        GUILayout.Label("Lower part");

        GUILayout.EndVertical();
        Repaint();
    }

    protected void ResizableSplit() {
        Rect cursorChangeRect = new Rect(0, currentScrollViewHeight, this.position.width, 5f);

        GUI.DrawTexture(cursorChangeRect, EditorGUIUtility.whiteTexture);
        EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeVertical);

        if (Event.current.type == EventType.mouseDown && cursorChangeRect.Contains(Event.current.mousePosition)) {
            resize = true;
        }
        if (resize) {
            currentScrollViewHeight = Event.current.mousePosition.y;
            cursorChangeRect.Set(cursorChangeRect.x, currentScrollViewHeight, cursorChangeRect.width, cursorChangeRect.height);
        }
        if (Event.current.type == EventType.MouseUp)
            resize = false;
    }
}