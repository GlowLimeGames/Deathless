using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue {
    [Serializable]
    public enum NodeType { LINE, CHOICE }

    [Serializable]
    public enum RepeatRestriction { NONE, SHOW_ONCE, DONT_SHOW }

    [Serializable]
    public class NodeData : MonoBehaviour {
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
        private RepeatRestriction restriction;
        public RepeatRestriction Restriction {
            get { return restriction; }
            set { restriction = value; }
        }

        [SerializeField]
        private Conditions conditions;
        public Conditions Conditions {
            get { return conditions; }
            set { conditions = value; }
        }
        
        [SerializeField]
        private Actions actions;
        public Actions Actions {
            get { return actions; }
            set { actions = value; }
        }

        [SerializeField]
        private string notes;
        public string Notes {
            get { return notes; }
            set { notes = value; }
        }

        public void Init(NodeType type, Transform parentObject) {
            this.type = type;
            Text = "Add text here";
            Conditions = gameObject.AddComponent<Conditions>();
            Actions = gameObject.AddComponent<Actions>();

            gameObject.transform.SetParent(parentObject);
            gameObject.name = "dialogue_nodedata";
        } 
    }
}