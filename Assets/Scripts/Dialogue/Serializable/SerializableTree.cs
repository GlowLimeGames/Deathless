using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue {
    [CreateAssetMenu(menuName = "Dialogue Tree")] [Serializable]
    public class SerializableTree : ScriptableObject {
        [SerializeField]
        private GameObject owner;
        [SerializeField] [HideInInspector]
        private List<SerializableNode> nodes;
        [SerializeField] [HideInInspector]
        private List<SerializableNode> links;
        
        [NonSerialized]
        private Dictionary<Node, SerializableNode> tempNodes;

        public void ExportTree (DialogueTree export) {
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

        public DialogueTree ImportTree () {
            DialogueTree imported = null;

            if (nodes != null && nodes.Count > 0 && links != null) {
                imported = new DialogueTree();
                imported.owner = owner;
                ImportChildNodes(imported.root, nodes[0]);
                ImportLinks();
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