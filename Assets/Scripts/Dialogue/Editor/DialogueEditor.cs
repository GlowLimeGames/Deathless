using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Dialogue;
using UnityEngine;
//
public class DialogueEditor : EditorWindow {
    protected Vector2 scrollPos = Vector2.zero;

    [SerializeField]
    private SerializableTree savedTree, lastSavedTree;
    private DialogueTree tree;
    private Dictionary<int, NodeGUI> nodes;
    private int nextID;
    private bool contextMenuShown;
    private Node copiedLink;
    private bool dirty = false;

    private List<BaseNode> forceExpandNodes;
    private BaseNode nodeToSelect;

    [SerializeField]
    private NodeData dataInEditor;

    [MenuItem("Window/Dialogue Editor")]
    public static void ShowWindow() {
        GetWindow(typeof(DialogueEditor));
    }

    void OnGUI() {
        GUILayout.BeginVertical();
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        EditorGUIUtility.hierarchyMode = true;
        EditorGUI.indentLevel++;

        GUI.SetNextControlName("DummyControl");
        GUI.Button(new Rect(0, 0, 0, 0), "", GUIStyle.none);

        savedTree = (SerializableTree)EditorGUILayout.ObjectField("Dialogue Tree", savedTree, typeof(SerializableTree), true);

        if (savedTree == null || (lastSavedTree != null && savedTree != lastSavedTree)) {
            Cleanup();
            tree = null;
            nodes = null;
        }
        if (savedTree != null) {
            if (GUILayout.Button("Save") && tree != null) {
                savedTree = savedTree.ExportInstance(tree);
                EditorUtility.SetDirty(savedTree);
                
                Debug.Log("Saved tree");
            }

            if (GUILayout.Button("Load")) {
                Cleanup();
                lastSavedTree = savedTree;
                tree = savedTree.InstantiateTree().ImportTree();
                if (tree == null) {
                    tree = DialogueTester.CreateTestTree(savedTree.gameObject.transform);
                    Debug.Log("Created new tree");
                }
            }
        }

        if (savedTree != null && tree != null) {
            dirty = true;
            if (nodes == null) {
                nodes = new Dictionary<int, NodeGUI>();
                nextID = 0;
            }
            NodeGUI.RenderNode(this, tree.root);

            NodeGUI gui = GetNodeAtPoint(Event.current.mousePosition);
            if (gui != null) {
                switch (Event.current.type) {
                    case EventType.MouseDown:
                        if (contextMenuShown) {
                            GUI.FocusControl("DummyControl");
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
                    if (focused.node.Data == null) { Debug.LogWarning("DATA IS NULL"); }

                    dataInEditor = focused.node.Data;
                    NodeEditor.Link = focused.node.isLink;

                    if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.Delete) {
                        focused.Remove(this);
                    }
                    else if (focused.node.isLink && Event.current.clickCount == 2) {
                        SelectNode(focused.node.GetOriginal());
                    }
                }
                else { dataInEditor = null; }

                if (dataInEditor != null) {
                    Selection.activeGameObject = dataInEditor.gameObject;
                    NodeEditor[] editors = Resources.FindObjectsOfTypeAll<NodeEditor>();
                    foreach (NodeEditor editor in editors) { editor.Repaint(); }
                }
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void Cleanup() {
        if (dirty == true) {
            foreach (SerializableTree tree in FindObjectsOfType<SerializableTree>()) {
                tree.CleanupTempInstance();
            }

            foreach (NodeData data in FindObjectsOfType<NodeData>()) {
                if (data.transform.parent == null) {
                    DestroyImmediate(data.gameObject);
                }
            }

            dirty = false;
        }
    }

    private void GenerateContextMenu(NodeGUI gui) {
        contextMenuShown = true;
        GenericMenu menu = new GenericMenu();

        if (!gui.node.isLink) {
            menu.AddItem(new GUIContent("Add Line"), false, AddLine, gui.node);
            menu.AddItem(new GUIContent("Add Choice"), false, AddChoice, gui.node);

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Copy Link"), false, CopyLink, gui.node);
            if (copiedLink != null) {
                menu.AddItem(new GUIContent("Paste Link"), false, PasteLink, gui.node);
            }

            if (gui.node.Parent != null) {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, RemoveNode, gui);
            }
        }
        else {
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

    private void AddLine(object obj) {
        if (((Node)obj).Data == null) { Debug.LogWarning("Attempting to add a line to a node with no data."); }
        ((Node)obj).AddNode(NodeType.LINE);
    }

    private void AddChoice(object obj) {
        ((Node)obj).AddNode(NodeType.CHOICE);
    }

    private void RemoveNode(object obj) {
        NodeGUI gui = (NodeGUI)obj;
        gui.Remove(this);
    }

    private void CopyLink(object obj) {
        copiedLink = (Node)obj;
    }

    private void PasteLink(object obj) {
        ((Node)obj).AddLink(copiedLink);
    }
    
    private class NodeGUI {
        public int id { get; private set; }
        public bool expanded { get; set; }
        public BaseNode node { get; private set; }
        Rect rect;

        public NodeGUI(BaseNode node) {
            this.node = node;
        }

        public void Remove(DialogueEditor editor) {
            node.Remove();
            editor.nodes.Remove(id);
            GUI.FocusControl("DummyControl");
            editor.Repaint();
        }

        public static void RenderNode(DialogueEditor editor, BaseNode node) {
            NodeGUI gui = editor.GetNodeGUI(node);

            if (gui != null && gui.node.Data == null) {
                gui.Remove(editor);
            }
            else {
                if (gui == null) {
                    gui = new NodeGUI(node);
                    gui.id = editor.nextID++;
                    editor.nodes.Add(gui.id, gui);
                }
                gui.RenderNode(editor);
                editor.forceExpandNodes = null;
            }
        }

        private void RenderNode(DialogueEditor editor) {
            GUI.SetNextControlName(id.ToString());
            bool isChoice = (node.Data.Type == NodeType.CHOICE);
            string text = node.Data.Text;
            if (text == "") { text = "<empty>"; }

            if (editor.forceExpandNodes != null) {
                if (editor.forceExpandNodes.Contains(node)) {
                    expanded = true;
                }
            }

            if (node.isLink || ((Node)node).Children.Count == 0) {
                EditorGUILayout.SelectableLabel(text, GetStyle(EditorStyles.label, isChoice, node.isLink), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                rect = GUILayoutUtility.GetLastRect();
            }
            else {
                expanded = EditorGUILayout.Foldout(expanded, text, GetStyle(EditorStyles.foldout, isChoice));
                rect = GUILayoutUtility.GetLastRect();
                if (expanded) {
                    EditorGUI.indentLevel++;
                    foreach (BaseNode child in ((Node)node).Children) {
                        RenderNode(editor, child);
                    }
                    EditorGUI.indentLevel--;
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