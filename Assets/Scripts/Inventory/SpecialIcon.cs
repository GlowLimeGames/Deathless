using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The icon the player can use to examine items in
/// their inventory.
/// </summary>
public class SpecialIcon : InventoryItem {
    /// <summary>
    /// Handle clicks to this icon. Should be called from the
    /// Button component on the gameObject.
    /// </summary>
    public void OnClick() {
        if (Inventory.SelectedItem != null) {
            Interact();
        }
        else {
            Inventory.SelectItem(this);
        }
    }
}