using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour {
    public bool isEmpty {
        get { return (item == null); }
    }

    private InventoryItem item;

    public bool SetItem(InventoryItem item) {
        if (!isEmpty) {
            Debug.LogError("Attempted to put an item in an occupied slot. Aborting.");
            return false;
        }
        else {
            this.item = item;
            item.transform.SetParent(transform, false);
            return true;
        }
    }

    public void Clear() {
        Destroy(item.gameObject);
        item = null;
    }

    public void MoveTo(InventorySlot slot) {
        if (slot.SetItem(item)) {
            item = null;
        }
    }

    public bool ItemEquals(InventoryItem item) {
        return this.item.Equals(item);
    }

    public void OnClick() {
        if (!isEmpty) {
            if (Inventory.isItemSelected()) {
                //Temporary, for testing
                Inventory.RemoveItem(item);
                Inventory.ClearSelection();
            }
            else {
                Inventory.SelectItem(item);
            }
        }
    }
}