using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An item that can be stored in the player's inventory.
/// </summary>
public class InventoryItem : GameItem {
    private Image image;

    [SerializeField]
    private Sprite cursorSprite;
    public Sprite CursorSprite { get; private set; }

    void Start() {
        image = gameObject.GetComponent<Image>();
        CursorSprite = (cursorSprite == null ? image.sprite : cursorSprite);
    }

    public override void ChangeSprite(Sprite sprite) {
        if (((InventoryItem)Instance).image != null) {
            ((InventoryItem)Instance).image.sprite = sprite;
        }
    }
}