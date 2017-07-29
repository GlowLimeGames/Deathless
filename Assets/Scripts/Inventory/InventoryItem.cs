using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An item that can be stored in the player's inventory.
/// </summary>
public class InventoryItem : GameItem {
    private Image image;

    void Start() {
        image = gameObject.GetComponent<Image>();
    }

    public override void ChangeSprite(Sprite sprite) {
        if (image != null) {
            image.sprite = sprite;
        }
    }
}