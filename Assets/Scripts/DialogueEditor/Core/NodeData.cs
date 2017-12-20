using System;
using UnityEngine;

namespace Dialogue {
    [Serializable]
    public enum NodeType { LINE, CHOICE }

    [Serializable]
    public enum RepeatRestriction { NONE, ONCE_PER_GAME, ONCE_PER_CONVO, DONT_SHOW }

    [RequireComponent(typeof(Actions))]
    [RequireComponent(typeof(Conditions))]
    [Serializable]
    public class NodeData : MonoBehaviour {
        [SerializeField]
        private int id;
        public int ID {
            get { return id; }
            set { id = value; }
        }

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
            get {
                if (conditions == null) {
                    conditions = gameObject.GetComponent<Conditions>();
                }
                if (conditions == null) {
                    conditions = gameObject.AddComponent<Conditions>();
                }
                return conditions;
            }
            private set { conditions = value; }
        }
        
        [SerializeField]
        private Actions actions;
        public Actions Actions {
            get {
                if (actions == null) {
                    actions = gameObject.GetComponent<Actions>();
                }
                if (actions == null) {
                    actions = gameObject.AddComponent<Actions>();
                }
                return actions;
            }
            private set { actions = value; }
        }

        [SerializeField]
        private string notes;
        public string Notes {
            get { return notes; }
            set { notes = value; }
        }

        [SerializeField]
        private bool endDialogue;
        public bool EndDialogue {
            get { return endDialogue; }
            set { endDialogue = value; }
        }

        [SerializeField]
        private bool randomizeChildren;
        public bool RandomizeChildren {
            get { return randomizeChildren; }
            set { randomizeChildren = value; }
        }

        public void Init(NodeType type, Transform parentObject) {
            this.type = type;
            Text = "Add text here";

            #if UNITY_EDITOR
            //Adjust so NodeData component is shown before Actions & Conditions.
            UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
            UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
            #endif

            gameObject.transform.SetParent(parentObject);
            gameObject.name = "dialogue_nodedata";
        }

        public bool ValidateComponents() {
            bool valid = (Actions != null && Conditions != null);

            if (!valid) { Debug.LogWarning("NodeData component validation failed: " + Text); }
            return valid;
        }
        
        public bool ValidateParent(NodeData parent) {
            bool valid = (gameObject.transform.parent == parent.gameObject.transform);

            if (!valid) {
                Debug.Log("NodeData has incorrect parent. Attempting to fix...");
                gameObject.transform.SetParent(parent.gameObject.transform);
                valid = (gameObject.transform.parent == parent.gameObject.transform);
            }
            if (!valid) { Debug.LogWarning("Failed to fix NodeData parentage: " + Text); }

            return valid;
        }
    }
}