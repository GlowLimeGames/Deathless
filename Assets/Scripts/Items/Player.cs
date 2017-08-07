using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the player character WorldItem.
/// </summary>
public class Player : WorldItem {
    /// <summary>
    /// Backing field for MovementEnabled.
    /// </summary>
    private bool movementEnabled = true;

    /// <summary>
    /// Backing field for UseItemDialogue.
    /// </summary>
    [SerializeField]
    private Dialogue.SerializableTree useItemDialogue;

    /// <summary>
    /// The current instance of the player in the scene.
    /// </summary>
    private static Player instance {
        get { return GameManager.Player; }
    }

    public override GameItem Instance {
        get { return instance; }
    }

    /// <summary>
    /// Whether the player should be able to move. Will be false
    /// if there is no player character in the scene.
    /// </summary>
    public static bool MovementEnabled {
        get {
            return (instance != null && instance.movementEnabled);
        }
        set {
            if (instance != null) {
                instance.movementEnabled = value;
            }
        }
    }

    /// <summary>
    /// Default dialogue tree for using objects on each other.
    /// </summary>
    public static Dialogue.SerializableTree UseItemDialogue {
        get {
            if (instance != null) {
                return instance.useItemDialogue;
            }
            else { return null; }
        }
    }

    /// <summary>
    /// Moves the player to the given point, but only if player movement
    /// is currently enabled.
    /// </summary>
    public override void MoveToPoint(Vector2 point, bool isDialogueAction = false) {
        if (MovementEnabled) { base.MoveToPoint(point, isDialogueAction); }
    }

    public override void OnTargetReached(Transform target) {
        base.OnTargetReached(target);

        if (InteractionTarget != null) {
            InteractionTarget.Interact();
        }
    }
}