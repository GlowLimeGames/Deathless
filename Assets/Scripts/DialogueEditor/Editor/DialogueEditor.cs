using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Dialogue;
using UnityEngine;

public class DialogueEditor : EditorWindow {
    private const float INDENT_SIZE = 20;
    private const string DUMMY_CONTROL = "DummyControl";

    private Vector2 scrollPos = Vector2.zero;
    private int indentLevel;

    [SerializeField]
    private SerializableTree savedTree, lastSavedTree;
    private DialogueTree tree;
    private Dictionary<int, NodeGUI> nodes;
    private int nextID;
    private bool contextMenuShown;
    private Node copiedNode;

    private List<BaseNode> forceExpandNodes;
    private BaseNode nodeToSelect;

    [SerializeField]
    private NodeData dataInEditor;

    [MenuItem("Window/Dialogue Editor")]
    public static void ShowWindow() {
        GetWindow<DialogueEditor>();
    }

    private void OnDestroy() { StaticCleanup(); }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptReload() {
        DialogueEditor[] dlgEditorWindows = Resources.FindObjectsOfTypeAll<DialogueEditor>();
        if (dlgEditorWindows.Length > 0) {
            dlgEditorWindows[0].Cleanup();
        }
        else { StaticCleanup(); }
    }

    void OnGUI() {
        GUILayout.BeginVertical();

        EditorGUIUtility.hierarchyMode = true;
        indentLevel = 1;

        GUI.SetNextControlName(DUMMY_CONTROL);
        GUI.Button(new Rect(0, 0, 0, 0), "", GUIStyle.none);

        savedTree = (SerializableTree)EditorGUILayout.ObjectField("Dialogue Tree", savedTree, typeof(SerializableTree), true);
        
        CheckTreeHasChanged();

        if (savedTree != null) {
            ShowSaveLoadButtons();

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            if (tree != null && tree.root.Data != null) {
                if (nodes == null) {
                    nodes = new Dictionary<int, NodeGUI>();
                    nextID = 0;
                }
                NodeGUI.RenderAllNodes(this, tree.root);
                forceExpandNodes = null;

                HandleMouseClick();

                if (focusedWindow == this) {
                    NodeGUI focused = GetNodeGUI(GUI.GetNameOfFocusedControl());
                    if (nodeToSelect != null) {
                        if (focused == GetNodeGUI(nodeToSelect)) {
                            nodeToSelect = null;
                        }
                        else {
                            SelectNode(nodeToSelect);
                        }
                    }
                    else if (focused != null && focused.node != null) {
                        HandleButtonPress(focused);
                    }
                    else { dataInEditor = null; }

                    if (dataInEditor != null) {
                        Selection.activeGameObject = dataInEditor.gameObject;
                    }
                }
            }
            GUILayout.EndScrollView();
        }
        GUILayout.EndVertical();
    }

    private bool CheckTreeHasChanged() {
        bool changed = (savedTree == null || (lastSavedTree != null && savedTree != lastSavedTree));
        if (changed) {
            Cleanup();
            tree = null;
            nodes = null;
        }
        return changed;
    }

    private void ShowSaveLoadButtons() {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save") && tree != null && tree.root.Data != null) {
            savedTree = savedTree.ExportInstance(tree);
            EditorUtility.SetDirty(savedTree);

            CalculateNodeIDs();

            Debug.Log("Saved tree");
        }

        if (GUILayout.Button("Load")) {
            Cleanup();
            lastSavedTree = savedTree;
            SerializableTree treeInstance = savedTree.InstantiateTree();
            tree = treeInstance.ImportTree();
            if (tree == null) {
                tree = DialogueTester.CreateTestTree(treeInstance.gameObject.transform);
                Debug.Log("Created new tree");
            }
        }

        GUILayout.EndHorizontal();
    }

    private void HandleMouseClick() {
        NodeGUI gui = GetNodeAtPoint(Event.current.mousePosition);
        if (gui != null) {
            switch (Event.current.type) {
                case EventType.MouseDown:
                    if (contextMenuShown) {
                        GUI.FocusControl(DUMMY_CONTROL);
                        dataInEditor = null;
                        contextMenuShown = false;
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (Event.current.button == 1) {
                        GenerateContextMenu(gui);
                        Event.current.Use();
                    }
                    break;
            }
        }
    }

    private void HandleButtonPress(NodeGUI focused) {
        if (focused.node.Data == null) { Debug.LogWarning("DATA IS NULL"); }

        dataInEditor = focused.node.Data;
        NodeEditor.Editable = !focused.node.isLink && focused.node.Parent != null;

        bool modifierHeld = (Application.platform == RuntimePlatform.OSXEditor) ? Event.current.command : Event.current.control;

        if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Delete) {
            focused.Remove(this);
            Event.current.Use();
        }
        else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.E && modifierHeld) {
            focused.ExpandAll(!focused.expanded, this);
            Event.current.Use();
        }
        else if (focused.node.isLink && Event.current.clickCount == 2) {
            SelectNode(focused.node.GetOriginal());
        }
    }

    private static void StaticCleanup() {
        foreach (SerializableTree tree in FindObjectsOfType<SerializableTree>()) {
            tree.CleanupTempInstance();
        }

        foreach (NodeData data in FindObjectsOfType<NodeData>()) {
            if (data.transform.parent == null) {
                DestroyImmediate(data.gameObject);
            }
        }
    }

    private void Cleanup() {
        StaticCleanup();
        GUI.FocusControl(DUMMY_CONTROL);
        nodes = null;
        GetWindow<DialogueEditor>().Repaint();
    }

    private static void CalculateNodeIDs() {
        int i = 0;

        foreach (NodeData data in Resources.FindObjectsOfTypeAll<NodeData>()) {
            data.ID = i;
            i++;
        }
    }

    private void GenerateContextMenu(NodeGUI gui) {
        contextMenuShown = true;
        GenericMenu menu = new GenericMenu();

        if (!gui.node.isLink) {
            menu.AddItem(new GUIContent("Add Line"), false, AddLine, gui.node);
            menu.AddItem(new GUIContent("Add Choice"), false, AddChoice, gui.node);

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Move Up"), false, MoveUp, gui.node);
            menu.AddItem(new GUIContent("Move Down"), false, MoveDown, gui.node);

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Copy"), false, CopyNode, gui.node);
            if (copiedNode != null) {
                menu.AddItem(new GUIContent("Move Here"), false, MoveNode, gui.node);
                menu.AddItem(new GUIContent("Paste Link"), false, PasteLink, gui.node);
            }

            if (gui.node.Parent != null) {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, RemoveNode, gui);
            }
        }
        else {
            menu.AddItem(new GUIContent("Move Up"), false, MoveUp, gui.node);
            menu.AddItem(new GUIContent("Move Down"), false, MoveDown, gui.node);

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Remove Link"), false, RemoveNode, gui);
        }

        menu.ShowAsContext();
    }

    private NodeGUI GetNodeAtPoint(Vector2 point) {
        if (nodes != null) {
            foreach (NodeGUI gui in nodes.Values) {
                if (gui.Contains(point)) {
                    return gui;
                }
            }
        }
        return null;
    }

    private NodeGUI GetNodeGUI(BaseNode node) {
        foreach (NodeGUI gui in nodes.Values) {
            if (gui.node == node) {
                return gui;
            }
        }
        return null;
    }

    private NodeGUI GetNodeGUI(string idString) {
        NodeGUI gui = null;
        int id;
        if (nodes != null && int.TryParse(idString, out id)) {
            if (nodes.ContainsKey(id)) {
                gui = nodes[id];
            }
        }
        return gui;
    }

    private void SelectNode(BaseNode node) {
        if (nodeToSelect != null) {
            NodeGUI gui = GetNodeGUI(node);
            GUI.FocusControl(gui.id.ToString());
        }
        else {
            RevealNode(node);
            nodeToSelect = node;
        }
    }

    /// <summary>
    /// Expands all ancester nodes of the given node.
    /// </summary>
    private void RevealNode(BaseNode node) {
        if (forceExpandNodes == null) {
            forceExpandNodes = new List<BaseNode>();
            RevealNode(node.Parent);
            Repaint();
        }
        else if (node != null) {
            forceExpandNodes.Add(node);
            RevealNode(node.Parent);
        }
    }

    /// <summary>
    /// Expands all children of the given node, recursively.
    /// </summary>
    private void RevealChildren(BaseNode node) {
        if (forceExpandNodes == null) {
            forceExpandNodes = new List<BaseNode>();
            RevealChildren(node);
            Repaint();
        }
        else {
            forceExpandNodes.Add(node);
            if (!node.isLink && ((Node)node).Children.Count > 0) {
                foreach (BaseNode child in ((Node)node).Children) {
                    RevealChildren(child);
                }
            }
        }
    }

    private void AddLine(object obj) {
        if (((Node)obj).Data == null) { Debug.LogWarning("Attempting to add a line to a node with no data."); }
        ((Node)obj).AddNode(NodeType.LINE);
    }

    private void AddChoice(object obj) {
        ((Node)obj).AddNode(NodeType.CHOICE);
    }

    private void MoveUp(object obj) {
        ((BaseNode)obj).ChangePosition(-1);
    }

    private void MoveDown(object obj) {
        ((BaseNode)obj).ChangePosition(1);
    }

    private void RemoveNode(object obj) {
        NodeGUI gui = (NodeGUI)obj;
        gui.Remove(this);
    }

    private void CopyNode(object obj) {
        copiedNode = (Node)obj;
    }

    private void MoveNode(object obj) {
        copiedNode.Move((Node)obj);
    }

    private void PasteLink(object obj) {
        ((Node)obj).AddLink(copiedNode);
    }
    
    private class NodeGUI {
        public int id { get; private set; }
        public bool expanded { get; private set; }
        public BaseNode node { get; private set; }
        Rect rect;

        public NodeGUI(BaseNode node) {
            this.node = node;
        }

        /// <summary>
        /// Expand or collapse this node and all its children, recursively.
        /// </summary>
        public void ExpandAll(bool expand, DialogueEditor editor) {
            if (!node.isLink && ((Node)node).Children.Count > 0) {
                if (expand) { editor.RevealChildren(node); }
                else {
                    expanded = expand;
                    foreach (BaseNode child in ((Node)node).Children) {
                        NodeGUI gui = editor.GetNodeGUI(child);
                        if (gui != null) { gui.ExpandAll(expand, editor); }
                    }
                }
            }
        }

        public void Remove(DialogueEditor editor) {
            node.Remove();

            if (!node.isLink) {
                foreach (Link link in ((Node)node).Links) {
                    editor.GetNodeGUI(link).DestroyGUI(editor);
                }
            }

            DestroyGUI(editor);
        }

        private void DestroyGUI(DialogueEditor editor) {
            editor.nodes.Remove(id);
            GUI.FocusControl(DUMMY_CONTROL);
            editor.Repaint();
        }

        public static void RenderAllNodes(DialogueEditor editor, BaseNode root) {
            List<NodeGUI> removeNodes = new List<NodeGUI>();

            RenderNode(editor, root, removeNodes);

            foreach (NodeGUI gui in removeNodes) {
                gui.Remove(editor);
            }
        }

        private static void RenderNode(DialogueEditor editor, BaseNode node, List<NodeGUI> removeNodes) {
            NodeGUI gui = editor.GetNodeGUI(node);

            if (gui != null && gui.node.Data == null) {
                Debug.Log("trying to render a node with null data. destroying...");
                removeNodes.Add(gui);
            }
            else {
                if (gui == null) {
                    gui = new NodeGUI(node);
                    gui.id = editor.nextID++;
                    editor.nodes.Add(gui.id, gui);
                }
                gui.RenderNode(editor, removeNodes);
            }
        }

        private void RenderNode(DialogueEditor editor, List<NodeGUI> removeNodes) {
            if (node.Data == null) {
                Debug.Log("trying to render a node with null data. destroying...");
                removeNodes.Add(this);
                return;
            }

            GUI.SetNextControlName(id.ToString());
            bool isChoice = (node.Data.Type == NodeType.CHOICE);
            string text = node.Data.Text;
            if (text == "") {
                if (node.Data.Notes == "") { text = "<empty>"; }
                else { text = "<<" + node.Data.Notes + ">>"; }
            }

            if (!node.isBranchComplete()) { text += "*"; }

            if (editor.forceExpandNodes != null && editor.forceExpandNodes.Contains(node)) {
                expanded = true;
            }

            bool terminal = (node.isLink || ((Node)node).Children.Count == 0);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(editor.indentLevel * INDENT_SIZE);

            GUIContent textContent = new GUIContent(text);
            GUIStyle style = terminal ? GetStyle(EditorStyles.label, isChoice, node.isLink) : GetStyle(EditorStyles.foldout, isChoice);
            rect = GUILayoutUtility.GetRect(textContent, style);
            style.fixedWidth = rect.width;

            if (terminal) {
                EditorGUI.SelectableLabel(rect, text, style);
                EditorGUILayout.EndHorizontal();
            }
            else {
                expanded = EditorGUI.Foldout(rect, expanded, textContent, style);
                EditorGUILayout.EndHorizontal();
                if (expanded) {
                    editor.indentLevel++;
                    foreach (BaseNode child in ((Node)node).Children) {
                        RenderNode(editor, child, removeNodes);
                    }
                    editor.indentLevel--;
                }
            }
        }

        private GUIStyle GetStyle(GUIStyle defaultStyle, bool isChoice, bool isLink = false) {
            GUIStyle style = new GUIStyle(defaultStyle);
            
            Color highlight = new Color(0.6f, 0.6f, 0.6f, 1);

            Color color = isChoice ? Color.blue : new Color(0.8f, 0, 0, 1);
            if (isLink) { style.fontStyle = FontStyle.Italic; }
            
            Texture2D tex = new Texture2D(style.focused.background.width, style.focused.background.height);
            Color[] pixels = tex.GetPixels();

            for (int i = 0; i < pixels.Length; i++) {
                pixels[i] = highlight;
            }

            tex.SetPixels(pixels);
            tex.Apply();

            style.normal.textColor = color;
            style.onNormal.textColor = color;

            style.focused.background = tex;
            style.onFocused.background = tex;
            style.focused.textColor = color;
            style.onFocused.textColor = color;

            style.active.background = tex;
            style.onActive.background = tex;
            style.active.textColor = color;
            style.onActive.textColor = color;

            return style;
        }

        public bool Contains(Vector2 point) {
            return rect.Contains(point);
        }
    }
}