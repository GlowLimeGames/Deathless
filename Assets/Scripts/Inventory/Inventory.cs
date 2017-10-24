using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles the player's inventory.
/// </summary>
public class Inventory : Manager<Inventory>, IPointerClickHandler {
    /// <summary>
    /// An ordered list of all inventory slots in the inventory.
    /// </summary>
    private List<InventorySlot> slots = new List<InventorySlot>();

    public static List<InventorySlot> Slots { get { return Instance.slots; } }

    /// <summary>
    /// Whether the inventory is currently open.
    /// </summary>
    public static bool isShown {
        get { return Instance != null && Instance.gameObject.activeInHierarchy; }
    }

    /// <summary>
    /// The inventory item that is currently selected.
    /// </summary>
    public static InventoryItem SelectedItem { get; private set; }

    private static InventoryItem lastSelectedItem;

    [SerializeField]
    private InventoryItem observeItem;

    public static InventoryItem ObserveItem { get; private set; }

    public static bool isObserveIconSelected {
        get { return (ObserveItem != null && SelectedItem == ObserveItem); }
    }
    
	public void Init () {
        SingletonInit();

        ObserveItem = observeItem;
        
        // Add inventory slots to list
        foreach (InventorySlot slot in transform.GetComponentsInChildren<InventorySlot>(true)) {
            Slots.Add(slot);
        }
	}

    /// <summary>
    /// Non-static function which calls the static function Show().
    /// Necessary to attach to Unity button OnClick().
    /// </summary>
    /// <param name="visible"></param>
    public void ShowInstance(bool visible) { Show(visible); }

    /// <summary>
    /// Open or close the inventory.
    /// </summary>
    public static void Show(bool visible) {
        Instance.gameObject.SetActive(visible);
        UIManager.OnShowUIElement(visible);

        if (isObserveIconSelected) {
            ClearSelection();
        }
    }

    /// <summary>
    /// Add a new item to the inventory.
    /// </summary>
    public static void AddItem(InventoryItem prefab) {
        InventoryItem item = Instantiate(prefab);
        item.name = prefab.name;

        for (int i = 0; item != null && i < Slots.Count; i++) {
            if (Slots[i].isEmpty) {
                Slots[i].SetItem(item);
                item = null;
            }
        }

        if (item != null) { Debug.LogWarning("Failed to add item to inventory: " + item); }
    }

    /// <summary>
    /// Remove the equivalent item from the inventory.
    /// </summary>
    public static void RemoveItem(InventoryItem prefab) {
        if (SelectedItem != null && SelectedItem.Equals(prefab)) {
            ClearSelection();
        }

        foreach (InventorySlot slot in Slots) {
            if (slot.ItemEquals(prefab)) {
                slot.Clear();
                CollapseItems(Slots.IndexOf(slot));
                break;
            }
        }
    }

    /// <summary>
    /// Collapse items in the inventory so there are no empty
    /// slots left between them.
    /// </summary>
    private static void CollapseItems(int startIndex = 0) {
        List<InventorySlot> emptySlots = new List<InventorySlot>();

        for (int i = startIndex; i < Slots.Count; i++) {
            if (!Slots[i].isEmpty && emptySlots.Count > 0) {
                Slots[i].MoveTo(emptySlots[0]);
                emptySlots.RemoveAt(0);
            }
            if (Slots[i].isEmpty) {
                emptySlots.Add(Slots[i]);
            }
        }
    }

    /// <summary>
    /// Select the given item.
    /// </summary>
    public static void SelectItem(InventoryItem item, bool temp = false) {
        if (temp) {
            lastSelectedItem = SelectedItem;
        }
        else {
            lastSelectedItem = null;

            if (item == null) { UIManager.ClearCustomCursor(); }
            else { UIManager.SetCustomCursor(item.CursorSprite); }
        }

        SelectedItem = item;
    }

    public static void RevertSelection() {
        if (lastSelectedItem != null) {
            SelectedItem = lastSelectedItem;
            lastSelectedItem = null;
        }
    }

    /// <summary>
    /// Clear the current item or icon selection.
    /// </summary>
    public static void ClearSelection(bool temp = false) {
        SelectItem(null, temp);
    }

    /// <summary>
    /// Check if the given item is in the inventory.
    /// </summary>
    public static bool HasItem(InventoryItem prefab) {
        bool hasItem = false;
        foreach (InventorySlot slot in Slots) {
            if (slot.ItemEquals(prefab)) {
                hasItem = true;
                break;
            }
        }
        return hasItem;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {
            Show(false);
        }
    }
}