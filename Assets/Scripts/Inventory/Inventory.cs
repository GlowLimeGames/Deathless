using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles the player's inventory.
/// </summary>
public class Inventory : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler {
    /// <summary>
    /// The instance of the inventory in the current scene.
    /// </summary>
    private static Inventory instance;

    /// <summary>
    /// An ordered list of all inventory slots in the inventory.
    /// </summary>
    private static List<InventorySlot> slots = new List<InventorySlot>();

    /// <summary>
    /// Whether the inventory is currently open.
    /// </summary>
    public static bool isShown {
        get { return instance.gameObject.activeInHierarchy; }
    }

    /// <summary>
    /// The inventory item that is currently selected.
    /// </summary>
    public static InventoryItem SelectedItem { get; private set; }

    /// <summary>
    /// Whether the player has an inventory item selected.
    /// </summary>
    public static bool isItemSelected {
        get { return (SelectedItem != null); }
    }
    
    /// <summary>
    /// Whether the player has selected the observe icon.
    /// </summary>
    public static bool ObserveIconSelected { get; private set; }
    
    /// <summary>
    /// How long before we should close the inventory upon cursor exit.
    /// </summary>
    private const float POINTER_EXIT_TIMEOUT = 0.3f;

    /// <summary>
    /// How long it's been since the cursor has exited the inventory.
    /// -1 indicates that the cursor never entered the inventory.
    /// float.MinValue indicates that the cursor is currently inside the inventory.
    /// </summary>
    private float pointerExitedCounter = -1;
    
	public void Init () {
        // Singleton
        if (instance == null) {
            instance = this;
        }
        else { Destroy(gameObject); }
        
        // Add inventory slots to list
        foreach (InventorySlot slot in transform.GetComponentsInChildren<InventorySlot>(true)) {
            slots.Add(slot);
        }
	}

    void Update () {
        CheckForPointerExitTimeout();
    } 

    /// <summary>
    /// Open or close the inventory.
    /// </summary>
    public static void Show(bool visible) {
        instance.gameObject.SetActive(visible);
        instance.pointerExitedCounter = -1;

        if (ObserveIconSelected) {
            ClearSelection();
        }
    }

    /// <summary>
    /// Add a new item to the inventory.
    /// </summary>
    public static void AddItem(InventoryItem prefab) {
        InventoryItem item = Instantiate(prefab);

        for (int i = 0; item != null && i < slots.Count; i++) {
            if (slots[i].isEmpty) {
                slots[i].SetItem(item);
                item = null;
            }
        }

        if (item != null) { Debug.LogWarning("Failed to add item to inventory: " + item); }
    }

    /// <summary>
    /// Remove the equivalent item from the inventory.
    /// </summary>
    public static void RemoveItem(InventoryItem prefab) {
        if (isItemSelected && SelectedItem.Equals(prefab)) {
            ClearSelection();
        }

        foreach (InventorySlot slot in slots) {
            if (slot.ItemEquals(prefab)) {
                slot.Clear();
                CollapseItems(slots.IndexOf(slot));
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

        for (int i = startIndex; i < slots.Count; i++) {
            if (!slots[i].isEmpty && emptySlots.Count > 0) {
                slots[i].MoveTo(emptySlots[0]);
                emptySlots.RemoveAt(0);
            }
            if (slots[i].isEmpty) {
                emptySlots.Add(slots[i]);
            }
        }
    }

    /// <summary>
    /// Select the given item.
    /// </summary>
    public static void SelectItem(InventoryItem item) {
        SelectedItem = item;
        ObserveIconSelected = false;
        Util.SetCursor(item.GetComponent<Image>().sprite);
    }

    /// <summary>
    /// Select the observe icon.
    /// </summary>
    public static void SelectObserveIcon(Sprite observeIcon) {
        SelectedItem = null;
        ObserveIconSelected = true;
        Util.SetCursor(observeIcon);
    }

    /// <summary>
    /// Clear the current item or icon selection.
    /// </summary>
    public static void ClearSelection() {
        SelectedItem = null;
        ObserveIconSelected = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    /// <summary>
    /// Check if the given item is in the inventory.
    /// </summary>
    public static bool HasItem(InventoryItem prefab) {
        bool hasItem = false;
        foreach (InventorySlot slot in slots) {
            if (slot.ItemEquals(prefab)) {
                hasItem = true;
                break;
            }
        }
        return hasItem;
    }

    /// <summary>
    /// Close the inventory after a buffer period once the cursor
    /// has left its bounds.
    /// </summary>
    private void CheckForPointerExitTimeout() {
        if (pointerExitedCounter > float.MinValue && Input.GetKeyUp(KeyCode.Mouse0)) {
            Show(false);
        }
        else if (pointerExitedCounter >= 0) {
            if (pointerExitedCounter >= POINTER_EXIT_TIMEOUT) {
                Show(false);
            }
            else {
                pointerExitedCounter += Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Registers when the cursor leaves the inventory.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData) {
        pointerExitedCounter = 0f;
    }

    /// <summary>
    /// Registers when the cursor enters the inventory.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData) {
        pointerExitedCounter = float.MinValue;
    }
}