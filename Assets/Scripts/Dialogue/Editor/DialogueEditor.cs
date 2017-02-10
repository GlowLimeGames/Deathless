using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Dialogue;
using UnityEngine;

public class DialogueEditor : SplitViewWindow {
    protected Vector2 scrollPosTree = Vector2.zero;
    protected Vector2 scrollPosEditor = Vector2.zero;

    [SerializeField]
    private SerializableTree savedTree, lastSavedTree;
    private DialogueTree tree;
    private Dictionary<int, NodeGUI> nodes { get; set; }
    private int nextID;
    private bool contextMenuShown;
    private Node copiedLink;

    [SerializeField]
    private NodeData dataInEditor;

    [MenuItem("Window/Dialogue Editor")]
    public static void ShowWindow() {
        GetWindow(typeof(DialogueEditor));
    }

    void OnGUI() {
        GUILayout.BeginVertical();
        scrollPosTree = GUILayout.BeginScrollView(scrollPosTree, GUILayout.Height(currentScrollViewHeight));

        EditorGUIUtility.hierarchyMode = true;
        EditorGUI.indentLevel++;

        GUI.SetNextControlName("DummyControl");
        GUI.Button(new Rect(0, 0, 0, 0), "", GUIStyle.none);

        savedTree = (SerializableTree)EditorGUILayout.ObjectField("Dialogue Tree", savedTree, typeof(SerializableTree), false);

        if (savedTree == null || (lastSavedTree != null && savedTree != lastSavedTree)) {
            tree = null;
            nodes = null;
        }
        if (savedTree != null) {
            if (GUILayout.Button("Save") && tree != null) {
                savedTree.ExportTree(tree);
                EditorUtility.SetDirty(savedTree);
                Debug.Log("Saved tree");
            }

            if (GUILayout.Button("Load")) {
                lastSavedTree = savedTree;
                tree = savedTree.ImportTree();
                if (tree == null) {
                    tree = DialogueTester.CreateTestTree();
                    Debug.Log("Created new tree");
                }
            }
        }
        
        if (savedTree != null && tree != null) {
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

            GUILayout.EndScrollView();
            ResizableSplit();
            scrollPosEditor = GUILayout.BeginScrollView(scrollPosEditor, GUILayout.Height(this.position.height - currentScrollViewHeight));
            
            NodeGUI focused = GetNodeGUI(GUI.GetNameOfFocusedControl());
            if (focused != null && focused.node != null) {
                dataInEditor = focused.node.Data;
                if (focused.node.Data == null) { Debug.Log("Data is null... :("); }
            }
            if (dataInEditor != null) {
                SerializedObject editor = new SerializedObject(this);
                SerializedProperty data = editor.FindProperty("dataInEditor");
                EditorGUILayout.PropertyField(data);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            Repaint();
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

    private void AddLine(object obj) {
        ((Node)obj).AddNode(NodeType.LINE);
    }

    private void AddChoice(object obj) {
        ((Node)obj).AddNode(NodeType.CHOICE);
    }

    private void RemoveNode(object obj) {
        NodeGUI gui = (NodeGUI)obj;
        nodes.Remove(gui.id);
        gui.node.Remove();
        GUI.FocusControl("DummyControl");
    }

    private void CopyLink(object obj) {
        copiedLink = (Node)obj;
    }

    private void PasteLink(object obj) {
        ((Node)obj).AddLink(copiedLink);
    }
    
    private class NodeGUI {
        public int id { get; private set; }
        bool expanded;
        public BaseNode node { get; private set; }
        Rect rect;

        public NodeGUI(BaseNode node) {
            this.node = node;
        }

        public static void RenderNode(DialogueEditor editor, BaseNode node) {
            NodeGUI gui = editor.GetNodeGUI(node);
            if (gui == null) {
                gui = new NodeGUI(node);
                gui.id = editor.nextID++;
                editor.nodes.Add(gui.id, gui);
            }
            gui.RenderNode(editor);
        }

        private void RenderNode(DialogueEditor editor) {
            GUI.SetNextControlName(id.ToString());

            if (node.isLink || ((Node)node).Children.Count == 0) {
                EditorGUILayout.SelectableLabel(node.Data.Text, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                //EditorGUILayout.LabelField(node.Data.Text);
                rect = GUILayoutUtility.GetLastRect();
            }
            else {
                expanded = EditorGUILayout.Foldout(expanded, node.Data.Text);
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

        public bool Contains(Vector2 point) {
            return rect.Contains(point);
        }
    }
}