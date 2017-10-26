using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue {
    public class SerializableTree : MonoBehaviour {
        [SerializeField]
        private GameObject owner;
        [SerializeField] [HideInInspector]
        private List<SerializableNode> nodes;
        [SerializeField] [HideInInspector]
        private List<SerializableNode> links;
        
        public GameObject Owner { get { return owner; } }
        
        [NonSerialized]
        private Dictionary<Node, SerializableNode> tempNodes;

        [NonSerialized]
        SerializableTree instance;

        public void CleanupTempInstance() {
            if (gameObject.activeInHierarchy) {
                if (Application.isPlaying) { Destroy(this.gameObject); }
                else { DestroyImmediate(this.gameObject); }
            }
            else if (instance != null && instance != this) {
                if (Application.isPlaying) { Destroy(instance.gameObject); }
                else { DestroyImmediate(instance.gameObject); }
            }
        }

        public SerializableTree ExportInstance (DialogueTree export) {
            if (instance == this) {
                ExportTree(export);
                return this;
            }
            else {
                instance.ExportTree(export);
                SerializableTree newPrefab = this;

                #if UNITY_EDITOR
                    newPrefab = UnityEditor.PrefabUtility.ReplacePrefab(instance.gameObject, this, UnityEditor.ReplacePrefabOptions.ConnectToPrefab).GetComponent<SerializableTree>();
                    newPrefab.instance = instance;
                #endif

                return newPrefab;
            }
        }

        private void ExportTree (DialogueTree export) {
            owner = export.owner;
            nodes = new List<SerializableNode>();
            links = new List<SerializableNode>();
            tempNodes = new Dictionary<Node, SerializableNode>();

            ExportNode(export.root);
            ExportLinks();
            tempNodes = null;
        }

        private SerializableNode ExportNode (BaseNode node) {
            SerializableNode newNode;

            if (node.isLink) {
                newNode = SerializableNode.NewLink(nodes.Count, (Link)node);
                nodes.Add(newNode);
                links.Add(newNode);
            }
            else {
                newNode = SerializableNode.NewNode(nodes.Count, node.Data);
                nodes.Add(newNode);
                tempNodes.Add((Node)node, newNode);
                
                foreach (BaseNode child in ((Node)node).Children) {
                    newNode.children.Add(nodes.Count);
                    ExportNode(child);
                }
            }
            return newNode;
        }

        private void ExportLinks () {
            foreach (SerializableNode node in links) {
                node.linkID = tempNodes[((Link)node.node).Original].id;
            }
        }

        public bool TryInstantiateTree (out SerializableTree tree) {
            if (gameObject.activeInHierarchy) {
                instance = this;
            }
            else {
                instance = Instantiate(this);
                instance.name = this.name;
            }

            tree = instance;
            return (!gameObject.activeInHierarchy);
        }

        public SerializableTree InstantiateTree () {
            if (gameObject.activeInHierarchy) { instance = this; }
            else {
                #if UNITY_EDITOR
                    instance = UnityEditor.PrefabUtility.InstantiatePrefab(this) as SerializableTree;
                #endif

                if (instance == null) { instance = Instantiate(this); }
                instance.name = this.name;
            }
            return instance;
        }

        public DialogueTree ImportTree () {
            DialogueTree imported = null;

            if (nodes != null && nodes.Count > 0 && links != null) {
                imported = new DialogueTree(nodes[0].data);
                imported.owner = owner;
                ImportChildNodes(imported.root, nodes[0]);
                ImportLinks();
                imported.ValidateTree();
            }

            return imported;
        }

        private void ImportChildNodes (Node node, SerializableNode parent) {
            foreach (int childID in parent.children) {
                SerializableNode child = nodes[childID];
                if (child.isLink) {
                    Link newLink = node.AddLink(null);
                    child.node = newLink;
                }
                else {
                    Node newNode = node.AddNode(child.data);
                    child.node = newNode;
                    ImportChildNodes(newNode, child);
                }
            }
        }

        private void ImportLinks () {
            foreach (SerializableNode node in links) {
                Node original = (Node)nodes[node.linkID].node;
                ((Link)nodes[node.id].node).Replace(original);
            }
        }
    }
    
    [Serializable]
    public class SerializableNode {
        [SerializeField]
        public int id;
        [SerializeField]
        public int linkID;
        [SerializeField]
        public NodeData data;
        [SerializeField]
        public List<int> children;

        [NonSerialized]
        public BaseNode node;

        [SerializeField]
        public bool isLink;

        private SerializableNode(int id, NodeData data, Link link) {
            this.id = id;
            this.data = data;
            this.node = link;
        }

        public static SerializableNode NewLink(int id, Link link) {
            SerializableNode newNode = new SerializableNode(id, null, link);
            newNode.isLink = true;
            return newNode;
        }

        public static SerializableNode NewNode(int id, NodeData data) {
            SerializableNode newNode = new SerializableNode(id, data, null);
            newNode.isLink = false;
            newNode.children = new List<int>();
            return newNode;
        }
    }
}