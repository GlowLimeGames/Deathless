using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue {
    [Serializable]
    public enum NodeType { LINE, CHOICE }

    [Serializable]
    public class NodeData : ScriptableObject {
        [SerializeField]
        private NodeType type;
        public NodeType Type {
            get { return type; }
        }
        
        [SerializeField]
        private GameObject speaker;
        public GameObject Speaker {
            get { return speaker; }
            set { speaker = value; }
        }

        [SerializeField]
        private string text;
        public string Text {
            get { return text; }
            set { text = value; }
        }

        [SerializeField]
        private Condition condition;
        public Condition Condition {
            get { return condition; }
            set { condition = value; }
        }
        
        [SerializeField]
        private UnityEvent action;
        public UnityEvent Action {
            get { return action; }
            set { action = value; }
        }

        [SerializeField]
        private string notes;
        public string Notes {
            get { return notes; }
            set { notes = value; }
        }

        public NodeData(NodeType type) {
            this.type = type;
            Text = "Add text here";
        }
    }
}