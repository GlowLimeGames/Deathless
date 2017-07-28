using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class for both inventory and world items.
/// </summary>
public class GameItem : MonoBehaviour {
    /// <summary>
    /// The item that the player has interacted with.
    /// </summary>
    public static GameItem InteractionTarget { get; protected set; }

    /// <summary>
    /// Backing field for ItemName.
    /// </summary>
    [SerializeField]
    private string itemName;

    /// <summary>
    /// The in-game name of this item.
    /// </summary>
    public string ItemName {
        get {
            if (itemName == null) { return "NULL"; }
            else { return itemName; }
        }
    }

    /// <summary>
    /// Backing field for Instance
    /// </summary>
    private GameItem instance = null;

    /// <summary>
    /// The instance of this GameItem in the current scene.
    /// </summary>
    protected virtual GameItem Instance {
        get {
            if (instance == null) {
                GameItem[] items = FindObjectsOfType<GameItem>();
                foreach (GameItem item in items) {
                    if (item.Equals(this)) {
                        instance = item;
                        break;
                    }
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Backing field for Dialogue property.
    /// </summary>
    [SerializeField]
    private Dialogue.SerializableTree dialogue;

    /// <summary>
    /// The unique dialogue tree for this item. Should be left null
    /// if the item has no significant dialogue of its own.
    /// </summary>
    public Dialogue.SerializableTree Dialogue {
        get { return dialogue; }
    }

    /// <summary>
    /// Inventory items which may be used on this item (this item's
    /// attached dialogue should have a branch for the combination).
    /// </summary>
    [SerializeField]
    private List<InventoryItem> validInteractions;

    /// <summary>
    /// Equality of game items will be based on their gameobject's name.
    /// Items must be of the same runtime type to be considered equal.
    /// </summary>
    public override bool Equals(object other) {
        bool equal = false;
        if (other.GetType() == typeof(GameItem)) {
            equal = Equals((GameItem)other);
        }
        return equal;
    }

    /// <summary>
    /// Equality of game items will be based on their gameobject's name.
    /// Items must be of the same runtime type to be considered equal.
    /// </summary>
    public bool Equals(GameItem other) {
        return ((other.gameObject.name == gameObject.name) &&
                (other.GetType() == GetType()));
    }

    /// <summary>
    /// Returns the hashcode of this object. (Hashcode override required
    /// to correspod to Equals() override.)
    /// </summary>
    public override int GetHashCode() {
        return gameObject.name.GetHashCode() + GetType().GetHashCode();
    }

    /// <summary>
    /// Trigger an interaction with this object.
    /// </summary>
    public void Interact() {
        if (!Inventory.isItemSelected || !Inventory.SelectedItem.Equals(this)) {
            Interact(!Inventory.isItemSelected);
        }
        else { CancelInteraction(); }
    }

    public static void CancelInteraction() {
        InteractionTarget = null;
    }

    /// <summary>
    /// Trigger an interaction with this object & specify the type
    /// of interaction.
    /// </summary>
    public void Interact(bool examine) {
        InteractionTarget = Instance;
        Dialogue.SerializableTree dlg = dialogue;

        if (!examine) {
            InventoryItem selected = Inventory.SelectedItem;

            if (!validInteractions.Contains(selected)) {
                if (GetType() == typeof(InventoryItem) && selected.validInteractions.Contains((InventoryItem)Instance)) {
                    // Reverse the selection/target order if necessary.
                    Inventory.SelectItem((InventoryItem)Instance);
                    dlg = null;
                    selected.Interact(examine);
                }
                else {
                    // This is the default dialogue for invalid combinations.
                    dlg = Player.UseItemDialogue;
                }
            }
        }

        if (dlg != null) { DialogueManager.StartDialogue(dlg); }
    }
}