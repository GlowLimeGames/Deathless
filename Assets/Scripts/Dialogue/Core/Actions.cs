using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue {
    [System.Serializable]
    public class Actions : NodeCache {
        [SerializeField]
        private UnityEvent actions;

        private static List<Actions> queue = new List<Actions>();
        private static Actions current {
            get {
                if (queue.Count > 0) { return queue[0]; }
                else { return null; }
            }
        }
        private int pendingActions = 0;

        public void Invoke() {
            if (actions != null) {
                queue.Add(this);
                if (current == this) { actions.Invoke(); }
                else { Debug.Log("Please note that simultaneous Actions are not permitted. Actions will be queued and run in succession."); }
            }
            else { DialogueManager.Continue(); }
        }

        void Update() {
            if (queue.Count > 0 && current.pendingActions == 0) {
                current.CompleteActions();
            }
        }

        private void CompleteActions() {
            queue.Remove(this);
            if (current != null) { current.actions.Invoke(); }
            else { DialogueManager.Continue(); }
        }

        public static void CompletePendingAction() {
            current.pendingActions--;
        }

        #region Actions

        [EnumAction(typeof(GlobalBool))]
        public void SetTrue(int i) {
            Globals.SetGlobal((GlobalBool)i, true);
        }

        [EnumAction(typeof(GlobalBool))]
        public void SetFalse(int i) {
            Globals.SetGlobal((GlobalBool)i, false);
        }

        public void SetInt(int i) {
            IntVar = i;
        }

        public void AddToInt(int i) {
            IntVar += i;
        }

        public void SetString(string s) {
            StringVar = s;
        }

        public void AddToInventory(InventoryItem item) {
            Inventory.AddItem(item);
        }

        public void RemoveFromInventory(InventoryItem item) {
            Inventory.RemoveItem(item);
        }

        public void SetSelectedItem (InventoryItem item) {
            Inventory.SelectItem(item);
        }

        public void ClearSelectedItem() {
            Inventory.ClearSelection();
        }

        public void RedirectDialogue (SerializableTree dlg) {
            DialogueManager.RedirectDialogue(dlg);
        }

        public void RemoveFromWorld(WorldItem item) {
            item.RemoveFromWorld();
        }

        public void EnableItem(WorldItem item) {
            item.Enable();
        }

        public void ChangeItemName(string name) {
            if (itemVar != null) { itemVar.SetName(name); }
            else {
                Debug.LogWarning("Unable to change item name to \"" + name + "\" " +
                    "- Note that you must cache an item (SetItemVar) before using this command.");
            }
        }

        public void ChangeItemSprite(Sprite sprite) {
            if (itemVar!= null) { itemVar.ChangeSprite(sprite); }
            else {
                Debug.LogWarning("Unable to set the sprite on this item. " +
                    "Note that you must cache an item (SetItemVar) before using this command.");
            }
        }

        /// <summary>
        /// Replaces the cached WorldItem with the given WorldItem in the game scene.
        /// </summary>
        public void ReplaceItemWith(WorldItem item) {
            if (itemVar != null) {
                if (itemVar.GetType() == typeof(WorldItem)) {
                    Vector3 pos = ((WorldItem)itemVar).GetPosition();
                    Instantiate(item, pos, Quaternion.identity);
                    ((WorldItem)itemVar).RemoveFromWorld();
                }
                else { Debug.LogWarning("Cannot place an inventory item in the world."); }
            }
            else { Debug.LogWarning("No WorldItem has been cached. Use SetItemVar to cache an item."); }
        }

        public void PlayAnimationAtItem(AnimationClip clip) {
            if (itemVar != null && itemVar.AnimController != null) {
                pendingActions++;
                itemVar.AnimController.PlayOneShot(clip, true);
            }
        }

        public void PlayAnimationAtPosition(AnimationClip clip) {
            if (transformVar != null) {
                pendingActions++;
                AnimController.PlayAnimAt(clip, GetPos(transformVar), true);
            }
        }

        public void MoveTo(Transform t) {
            WorldItem item = itemVar as WorldItem;
            if (item != null) {
                pendingActions++;
                item.MoveToPoint(GetPos(t), true);
            }
        }

        public void TeleportTo(Transform t) {
            WorldItem item = itemVar as WorldItem;
            if (item != null) {
                item.TeleportToPoint(GetPos(t));
            }
        }

        private Vector3 GetPos(Transform t) {
            Vector3 pos = t.position;

            WorldItem item = t.GetComponent<WorldItem>();
            if (item != null) { pos = item.GetPosition(); }

            return pos;
        }

        #endregion
    }
}