using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue {
    public class DialogueTree {
        public Node root;
        public GameObject owner;

        public DialogueTree() {
            root = Node.CreateRoot();
        }
    }
    
    public abstract class BaseNode {
        public Node Parent { get; private set; }
        public NodeData Data { get; private set; }
        public abstract bool isLink { get; }

        public BaseNode(Node parent, NodeData data) {
            Parent = parent;
            Data = data;
            if (Parent != null) {
                Parent.Children.Add(this);
            }
        }

        public virtual void Replace(Node node) {
            Data = node.Data;
        }

        public void Move(Node newParent) {
            Parent.Children.Remove(this);
            Parent = newParent;
            Parent.Children.Add(this);
        }

        public virtual void Remove() {
            Parent.Children.Remove(this);
        }
    }
    
    public class Link : BaseNode {
        public Node Original { get; private set; }
        public override bool isLink { get { return true; } }

        private Link (Node parent) : base(parent, null) { }

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

        public static Node CreateRoot() {
            Node node = new Node(null, new NodeData(NodeType.LINE));
            node.Data.Text = "<root>";
            return node;
        }

        public Node AddNode(NodeType type) {
            return new Node(this, new NodeData(type));
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
            }
        }

        private void ClearLinks() {
            while (Links.Count > 0) {
                Links[0].Remove();
            }
            foreach (BaseNode child in Children) {
                if (child.isLink) { child.Remove(); }
                else { ((Node)child).ClearLinks(); }
            }
        }
    }
}