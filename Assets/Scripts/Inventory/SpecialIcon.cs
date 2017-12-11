using UnityEngine;

/// <summary>
/// The icon the player can use to examine items in
/// their inventory.
/// </summary>
public class SpecialIcon : InventoryItem {
    [SerializeField]
    private string SELECT_SFX;

    /// <summary>
    /// Handle clicks to this icon. Should be called from the
    /// Button component on the gameObject.
    /// </summary>
    public void OnClick() {
        if (Inventory.SelectedItem != null) {
            Interact();
        }
        else {
            Inventory.SelectItem(this);
            if (SELECT_SFX != null && SELECT_SFX != "") {
                AudioController.PlayEvent(SELECT_SFX);
            }
        }
    }
}