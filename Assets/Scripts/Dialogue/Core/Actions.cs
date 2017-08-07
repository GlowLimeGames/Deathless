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

        private Node currentNode;

        public bool Invoke(Node currentNode) {
            if (actions != null && actions.GetPersistentEventCount() > 0) {
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
            if (queue.Count > 0 && current == this && pendingActions == 0) {
                CompleteActions();
            }
        }

        private void CompleteActions() {
            queue.Remove(this);
            if (current != null) { current.actions.Invoke(); }
            else { DialogueManager.Continue(currentNode); }
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

        public void InstantiateItemAtPosition(Transform t) {
            WorldItem item = itemVar as WorldItem;
            if (item != null) {
                Instantiate(item, GetPos(t), Quaternion.identity).gameObject.name = item.gameObject.name; ;
            }
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
                    Instantiate(item, GetPos(itemVar.transform), Quaternion.identity).gameObject.name = item.gameObject.name;
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

        public void FadeIn() {
            pendingActions++;
            UIManager.FadeIn(true);
        }

        public void FadeOut() {
            pendingActions++;
            UIManager.FadeOut(true);
        }

        public void GhostFadeIn(WorldItem ghost) {
            if (ghost.hasInstance) {
                GhostFadeAnim anim = ghost.Instance.gameObject.GetComponent<GhostFadeAnim>();
                if (anim != null) {
                    pendingActions++;
                    anim.StartFadeIn(true);
                }
            }
        }

        public void GhostFadeOut(WorldItem ghost) {
            if (ghost.hasInstance) {
                GhostFadeAnim anim = ghost.Instance.gameObject.GetComponent<GhostFadeAnim>();
                if (anim != null) {
                    pendingActions++;
                    anim.StartFadeOut(true);
                }
            }
        }

        #endregion
    }
}