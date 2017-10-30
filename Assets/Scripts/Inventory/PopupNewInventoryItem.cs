using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupNewInventoryItem : InventoryItem {
    private static InventoryItem newItem;
    private static SpriteRenderer spriteRenderer;

    public static void GetNewItem(InventoryItem prefab) {
        newItem = Instantiate(prefab);
        spriteRenderer = newItem.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = prefab.CursorSprite;
        

    }
	
}
