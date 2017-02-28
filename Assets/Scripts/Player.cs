using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : WorldItem {
    private bool movementEnabled = true;
    [SerializeField]
    private Dialogue.SerializableTree examineDialogue, useItemDialogue;

    private static Player instance {
        get { return GameManager.Player; }
    }

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

    public static Dialogue.SerializableTree ExamineDialogue {
        get {
            if (instance != null) {
                return instance.examineDialogue;
            }
            else { return null; }
        }
    }

    public static Dialogue.SerializableTree UseItemDialogue {
        get {
            if (instance != null) {
                return instance.useItemDialogue;
            }
            else { return null; }
        }
    }

    public override void MoveToPoint(Vector2 point, float speed = DEFAULT_SPEED) {
        if (MovementEnabled) { base.MoveToPoint(point, speed); }
    }
}