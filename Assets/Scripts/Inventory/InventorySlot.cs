using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A slot for a single item in the player's inventory.
/// </summary>
public class InventorySlot : Hoverable {
    /// <summary>
    /// Whether there is anything in this slot.
    /// </summary>
    public bool isEmpty {
        get { return (item == null); }
    }

    /// <summary>
    /// The InventoryItem in this slot.
    /// </summary>
    private InventoryItem item;

    /// <summary>
    /// Put the given item in this slot. (Will not work
    /// if the slot is already occupied.)
    /// </summary>
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

    /// <summary>
    /// Empty this slot.
    /// </summary>
    public void Clear() {
        Destroy(item.gameObject);
        item = null;
    }

    /// <summary>
    /// Move this slot's InventoryItem into the given slot.
    /// </summary>
    /// <param name="slot"></param>
    public void MoveTo(InventorySlot slot) {
        if (slot.SetItem(item)) {
            item = null;
        }
    }

    /// <summary>
    /// Whether this slot's InventoryItem is the same as the given item.
    /// </summary>
    public bool ItemEquals(InventoryItem item) {
        return this.item != null && this.item.Equals(item);
    }

    /// <summary>
    /// Handles clicks on this slot. Should be called from the Button
    /// component of the slot's gameObject.
    /// </summary>
    public void OnClick() {
        if (!isEmpty) {
            if (Inventory.SelectedItem != null) {
                item.Interact();
            }
            else {
                Inventory.SelectItem(item);
            }
        }
    }

    public override void OnHoverEnter() {
        if (!isEmpty) {
            item.OnHoverEnter();
        }
    }

    public override void OnHoverExit() {
        if (!isEmpty) { item.OnHoverExit(); }
    }
}