using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The icon the player can use to examine items in
/// their inventory.
/// </summary>
public class ObserveIcon : MonoBehaviour {
    /// <summary>
    /// The sprite to change the cursor to when observing items.
    /// </summary>
    [SerializeField]
    private Sprite observeIcon;

    /// <summary>
    /// Handle clicks to this icon. Should be called from the
    /// Button component on the gameObject.
    /// </summary>
    public void OnClick() {
        if (Inventory.isItemSelected) {
            Inventory.SelectedItem.Interact(true);
        }
        else {
            Inventory.SelectObserveIcon(observeIcon);
        }
    }
}