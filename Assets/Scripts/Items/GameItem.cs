using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class for both inventory and world items.
/// </summary>
public abstract class GameItem : Hoverable {
    /// <summary>
    /// Backing field for IsAnimate
    /// </summary>
    [SerializeField]
    private bool isAnimate;

    /// <summary>
    /// Property for whether this GameItem is inanimate or not
    /// </summary>
    public bool IsAnimate {
        get { return Instance.isAnimate; }
        private set { Instance.isAnimate = value; }
    }

    /// <summary>
    /// The item that the player has interacted with.
    /// </summary>
    public static GameItem InteractionTarget { get; protected set; }
   
    /// <summary>
    /// Backing field for DisplayName.
    /// </summary>
    [SerializeField]
    private string displayName;

    /// <summary>
    /// The in-game name of this item.
    /// </summary>
    public string DisplayName {
        get {
            if (Instance.displayName == null) { return "NULL"; }
            else { return Instance.displayName; }
        }
        private set { Instance.displayName = value; }
    }

    private AnimController animController;

    public AnimController AnimController {
        get {
            if (Instance.animController == null) {
                Instance.animController = Instance.GetComponent<AnimController>();
            }
            return Instance.animController;
        }
    }

    /// <summary>
    /// The instance of this GameItem in the current scene.
    /// </summary>
    public virtual new GameItem Instance {
        get { return (GameItem)base.Instance; }
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

    protected override SceneObject GetInstance() {
        SceneObject[] objs = Util.FindObjectsOfType<SceneObject>(true);
        foreach (SceneObject obj in objs) {
            if (obj.Equals(this)) {
                return obj;
            }
        }
        return null;
    }

    public override void OnHoverEnter() {
        UIManager.SetHoverText(DisplayName);
        CursorUtil.SetInteractionCursor(true);
    }

    public override void OnHoverExit() {
        UIManager.ClearHoverText();
        CursorUtil.SetInteractionCursor(false);
    }

    public void Enable() {
        Instance.gameObject.SetActive(true);
    }

    public void Disable() {
        Instance.gameObject.SetActive(false);
    }

    /// <summary>
    /// Trigger an interaction with this object.
    /// </summary>
    public virtual void Interact() {
        InteractionTarget = Instance;
        Dialogue.SerializableTree dlg = dialogue;

        if (Inventory.SelectedItem != null && !Inventory.isObserveIconSelected) {
            InventoryItem selected = Inventory.SelectedItem;

            if (selected.Equals(this)) { dlg = null; }
            else if (this.Equals(Inventory.ObserveItem)) {
                dlg = null;
                Inventory.ClearSelection(true);
                selected.Interact();
            }
            else if (!validInteractions.Contains(selected)) {
                if (GetType() == typeof(InventoryItem) && selected.validInteractions.Contains((InventoryItem)Instance)) {
                    dlg = null;
                    ReverseInteraction(selected);
                }
                else {
                    // This is the default dialogue for invalid combinations.
                    dlg = Player.UseItemDialogue;
                }
            }
        }

        if (dlg != null) { DialogueManager.StartDialogue(dlg); }
    }

    private void ReverseInteraction(InventoryItem newTarget) {
        Inventory.SelectItem((InventoryItem)Instance, true);
        newTarget.Interact();
    }

    public static void CancelInteraction() {
        InteractionTarget = null;
    }

    public void SetName(string s) {
        DisplayName = s;
    }

    public abstract void ChangeSprite(Sprite sprite);
}