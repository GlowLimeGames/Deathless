using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue {
    /// <summary>
    /// Designed to be able to allow you to have things happen in the game with dialogue.
    /// Used with unity events (Unity is pretty limited with their event system,
    /// so we expand on it with this script).
    /// </summary>
    [System.Serializable]
    public class ActionBase : NodeCache {
        /// <summary>
        /// Holds ALL OF THE ACTIONS for a node of dialogue.
        /// </summary>
        [SerializeField]
        private UnityEvent actions;

        /// <summary>
        /// Queue used to handle if game tries to trigger multiple trigger at the same time.
        /// </summary>
        private static List<ActionBase> queue = new List<ActionBase>();

        /// <summary>
        /// Returns the instance of Actions that has its actions currently running.
        /// </summary>
        private static ActionBase current {
            get {
                if (queue.Count > 0) { return queue[0]; }
                else { return null; }
            }
        }

        protected int pendingActions = 0;

        /// <summary>
        /// The current node of dialogue running. Allows us to resume dialogue
        /// after completing actions.
        /// </summary>
        private Node currentNode;

        /// <summary>
        /// Trigger all the actions. Note: not all actions needs the dialogue to pause, so we have stuff in the code handling that.
        /// </summary>
        public bool Invoke(Node currentNode) {
            if (actions != null && actions.GetPersistentEventCount() > 0 && !queue.Contains(this)) {
                this.currentNode = currentNode;
                queue.Add(this);
                if (current == this) { actions.Invoke(); }
                else { Debug.Log("Please note that simultaneous Actions are not permitted. Actions will be queued and run in succession."); }
                return true;
            }
            else {
                return DialogueManager.Continue(currentNode);
            }
        }

        void Update() {
            if (current == this && pendingActions == 0) {
                CompleteActions();
            }
        }

        private void OnDestroy() {
            if (current == this) { CompleteActions(); }
        }

        private void CompleteActions() {
            queue.Remove(this);
            if (current != null) { current.actions.Invoke(); }
            else { DialogueManager.Continue(currentNode); }
        }

        public static void CompletePendingAction() {
            current.pendingActions--;
        }
    }
}