using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler {
    private static Inventory instance;
    private static List<InventorySlot> slots = new List<InventorySlot>();
    private static InventoryItem selectedItem;
    
    private const float POINTER_EXIT_TIMEOUT = 0.3f;
    private float pointerExitedCounter = float.MinValue;
    
	public void Init () {
        if (instance == null) {
            instance = this;
        }
        else { Destroy(gameObject); }
        
        foreach (InventorySlot slot in transform.GetComponentsInChildren<InventorySlot>(true)) {
            slots.Add(slot);
        }
	}

    void Update () {
        CheckForPointerExitTimeout();
    } 

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

    public static void RemoveItem(InventoryItem prefab) {
        foreach (InventorySlot slot in slots) {
            if (slot.ItemEquals(prefab)) {
                slot.Clear();
                CollapseItems(slots.IndexOf(slot));
                break;
            }
        }
    }

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

    public static void SelectItem(InventoryItem item) {
        selectedItem = item;

        Texture2D texture = Util.CreateCursorTexture(item.GetComponent<Image>().sprite);
        Vector2 hotspot = new Vector2(texture.width / 2, texture.height / 2);
        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
    }

    public static void ClearSelection() {
        selectedItem = null;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public static bool HasItem(InventoryItem prefab) {
        return false;
    }

    public static bool isItemSelected() {
        return (selectedItem != null);
    }

    public static bool isItemSelected(InventoryItem prefab) {
        bool equal = false;
        if (isItemSelected()) {
            equal = selectedItem.Equals(prefab);
        }
        return equal;
    }

    private void CheckForPointerExitTimeout() {
        if (pointerExitedCounter != float.MinValue) {
            if (pointerExitedCounter >= POINTER_EXIT_TIMEOUT) {
                gameObject.SetActive(false);
                pointerExitedCounter = float.MinValue;
            }
            else {
                pointerExitedCounter += Time.deltaTime;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        pointerExitedCounter = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        pointerExitedCounter = float.MinValue;
    }
}