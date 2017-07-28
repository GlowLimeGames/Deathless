using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue {
    [System.Serializable]
    public class Actions : NodeCache {
        [SerializeField]
        private UnityEvent actions;

        public void Invoke() {
            if (actions != null) { actions.Invoke(); }
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

        public void ChangeItemName(string name) {
            if (itemVar != null) { itemVar.DisplayName = name; }
            else {
                Debug.LogWarning("Unable to change item name to \"" + name + "\" " +
                    "- Note that you must cache an item (SetItemVar) before using this command.");
            }
        }

        #endregion
    }
}