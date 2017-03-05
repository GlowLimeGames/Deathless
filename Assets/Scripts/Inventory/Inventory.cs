using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler {
    private static Inventory instance;
    private static List<InventorySlot> slots = new List<InventorySlot>();

    public static bool isShown {
        get { return instance.gameObject.activeInHierarchy; }
    }

    public static InventoryItem SelectedItem { get; private set; }
    public static bool isItemSelected {
        get { return (SelectedItem != null); }
    }
    
    public static bool ObserveIconSelected { get; private set; }
    
    private const float POINTER_EXIT_TIMEOUT = 0.3f;
    private float pointerExitedCounter = -1;
    
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

    public static void Show(bool visible) {
        instance.gameObject.SetActive(visible);
        instance.pointerExitedCounter = -1;

        if (ObserveIconSelected) {
            ClearSelection();
        }
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
        SelectedItem = item;
        ObserveIconSelected = false;
        Util.SetCursor(item.GetComponent<Image>().sprite);
    }

    public static void SelectObserveIcon(Sprite observeIcon) {
        SelectedItem = null;
        ObserveIconSelected = true;
        Util.SetCursor(observeIcon);
    }

    public static void ClearSelection() {
        SelectedItem = null;
        ObserveIconSelected = false;
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

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

    public void OnPointerExit(PointerEventData eventData) {
        pointerExitedCounter = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        pointerExitedCounter = float.MinValue;
    }
}