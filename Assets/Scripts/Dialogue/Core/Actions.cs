using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue {
    [System.Serializable]
    public class Actions : MonoBehaviour {
        [SerializeField]
        private UnityEvent actions;

        public void Invoke() {
            if (actions != null) { actions.Invoke(); }
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

        public void PickUpInteractionTarget () {
            if (GameItem.InteractionTarget.GetType() == typeof(WorldItem)) {
                WorldItem wItem = (WorldItem)GameItem.InteractionTarget;
                wItem.PickUp();
            }
        }
    }
}