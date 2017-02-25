using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {
    private static Inventory instance;
    private static List<InventorySlot> slots;
    private static InventoryItem selectedItem;

	// Use this for initialization
	void Start () {
        if (instance == null) {
            instance = FindObjectOfType<Inventory>();
        }
        else { Destroy(gameObject); }

		// Find and store InventorySlot children
        // of this object
	}

    public static void AddItem(InventoryItem item) {

    }

    public static void RemoveItem(InventoryItem item) {

    }

    private static void CollapseItems() {
        // Rearrange items so there are no
        // empty slots between items.
    }

    public static void SelectItem(InventoryItem item) {
        selectedItem = item;
    }

    public static void ClearSelection() {
        selectedItem = null;
    }

    public static bool HasItem(InventoryItem item) {
        return false;
    }

    public static bool ItemSelected(InventoryItem item) {
        // This should return true if the selected InventoryItem
        // is an instance of the given prefab InventoryItem,
        // even if it's not the exact same object.
        // Maybe use gameObject.name
        return item == selectedItem;
    }
}