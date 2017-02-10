using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour {
    private InventoryItem item;

    public void SetItem(InventoryItem item) {
        // Instantiate the item as a child
        // & set this.item to that child
    }

    public void OnClick() {
        if (item != null) {
            Inventory.SelectItem(item);
        }
    }
}