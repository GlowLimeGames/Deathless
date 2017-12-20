using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue {
    public class DialogueTree {
        public Node root;
        public GameObject owner;

        public DialogueTree(Transform parentObject) {
            root = Node.CreateRoot(parentObject);
        }

        public DialogueTree(NodeData rootData) {
            root = Node.CreateRoot(rootData);
        }

        public bool ValidateTree() {
            return root.ValidateNodeRecursively();
        }
    }
    
    public abstract class BaseNode {
        public Node Parent { get; private set; }
        public NodeData Data { get; private set; }
        public abstract bool isLink { get; }
        
        private BaseNode(Node parent) {
            Parent = parent;
            if (Parent != null) {
                Parent.Children.Add(this);
            }
        }

        protected BaseNode(Node parent, NodeType type) : this(parent) {
            GameObject data = new GameObject();
            Data = data.AddComponent<NodeData>();
            if (Data == null) { Debug.LogWarning("Data is null???"); }

            if (Parent != null) {
                if (Parent.Data == null) { Debug.LogWarning("Parent data is null??"); }
                Data.Init(type, Parent.Data.gameObject.transform);
            }
            else {
                Data.Init(type, null);
            }
        }

        protected BaseNode(Node parent, NodeData data) : this(parent) {
            Data = data;
        }

        public bool ValidateNodeRecursively(bool valid = true) {
            valid = ValidateNode(valid);
            
            if (!isLink && ((Node)this).Children != null) {
                foreach (BaseNode child in ((Node)this).Children) {
                    valid = child.ValidateNodeRecursively(valid);
                }
            }

            return valid;
        }

        private bool ValidateNode(bool valid) {
            if (Data == null) {
                valid = false;
                Debug.LogWarning("Node validation failed: node does not have any data.");
            }
            else {
                if (!Data.ValidateComponents()) {
                    valid = false;
                }

                if (!isLink) {
                    if (Data.EndDialogue && ((Node)this).Children != null && ((Node)this).Children.Count > 0) {
                        Data.EndDialogue = false;
                        Debug.Log("Please note! Dialogue nodes with children are NOT end nodes and should not be marked as such! Fixing...");
                    }
                    if (Parent != null && Parent.Data != null && !Data.ValidateParent(Parent.Data)) {
                        valid = false;
                    }
                }
            }

            return valid;
        }

        public Node GetOriginal() {
            Node original;
            if (isLink) {
                original = ((Link)this).Original;
            }
            else {
                original = (Node)this;
            }
            return original;
        }

        public virtual void Replace(Node node) {
            Data = node.Data;
        }

        public void Move(Node newParent) {
            Parent.Children.Remove(this);
            Parent = newParent;
            Parent.Children.Add(this);

            if (Data != null && Parent.Data != null) {
                if (!isLink) { Data.gameObject.transform.SetParent(Parent.Data.gameObject.transform); }
            }
            else {
                Debug.LogWarning("Failed to properly move data object " + Data + " to parent object " + Parent.Data + "./n" +
                                 "Move incomplete and likely buggy.");
            }
        }

        public void ChangePosition(int change) {
            int index = Parent.Children.IndexOf(this);
            int target = index + change;

            target = Mathf.Clamp(target, 0, Parent.Children.Count - 1);
            
            if (target != index) {
                Parent.Children.Remove(this);
                Parent.Children.Insert(target, this);
            }
        }

        public virtual void Remove() {
            Parent.Children.Remove(this);
        }

        public bool isBranchComplete() {
            bool complete = true;

            if (!isLink && !Data.EndDialogue) {
                Node node = (Node)this;

                if (node.Children.Count == 0) {
                    complete = false;
                }

                for (int i = 0; complete == true && i < node.Children.Count; i++) {
                    if (!node.Children[i].isBranchComplete()) {
                        complete = false;
                    }
                }
            }

            return complete;
        }
    }
    
    public class Link : BaseNode {
        public Node Original { get; private set; }
        public override bool isLink { get { return true; } }

        private Link(Node parent) : base(parent, null) { }

        private Link(Node parent, Node original) : base(parent, original.Data) {
            Original = original;
            Original.Links.Add(this);
        }

        public static Link Add(Node parent, Node original) {
            return (original == null) ?
                new Link(parent) : new Link(parent, original);
        }

        public override void Replace(Node node) {
            base.Replace(node);
            if (Original != null) {
                Original.Links.Remove(this);
            }
            Original = node;
            Original.Links.Add(this);
        }

        public override void Remove() {
            base.Remove();
            if (Original != null) { Original.Links.Remove(this); }
        }
    }
    
    public class Node : BaseNode {
        public List<BaseNode> Children { get; private set; }
        public List<Link> Links { get; private set; }
        public override bool isLink { get { return false; } }

        private Node(Node parent, NodeData data) : base(parent, data) {
            Children = new List<BaseNode>();
            Links = new List<Link>();
        }

        private Node(Node parent, NodeType type) : base(parent, type) {
            Children = new List<BaseNode>();
            Links = new List<Link>();
        }

        public static Node CreateRoot(Transform parentObject) {
            Node node = new Node(null, NodeType.LINE);
            node.Data.Text = "<root>";
            node.Data.gameObject.transform.SetParent(parentObject);
            return node;
        }

        public static Node CreateRoot(NodeData rootData) {
            Node node = new Node(null, rootData);
            return node;
        }

        public Node AddNode(NodeType type) {
            return new Node(this, type);
        }

        public Node AddNode(NodeType type, string text) {
            Node child = AddNode(type);
            child.Data.Text = text;
            return child;
        }

        public Node AddNode(NodeData data) {
            return new Node(this, data);
        }

        public Link AddLink(Node original) {
            return Link.Add(this, original);
        }

        public override void Remove() {
            if (Parent == null) { Debug.LogWarning("Root node cannot be removed."); }
            else {
                base.Remove();
                ClearLinks();
                while (Children.Count > 0) {
                    Children[0].Remove();
                }
                if (Data != null) { GameObject.DestroyImmediate(Data.gameObject); }
            }
        }

        private void ClearLinks() {
            while (Links.Count > 0) {
                Links[0].Remove();
            }
        }
    }
}