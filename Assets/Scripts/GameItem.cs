using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItem : MonoBehaviour {
    public static GameItem InteractionTarget { get; private set; }

    [SerializeField]
    private Dialogue.SerializableTree dialogue;
    public Dialogue.SerializableTree Dialogue {
        get { return dialogue; }
    }

    public override bool Equals(object other) {
        bool equal = false;
        if (other.GetType() == typeof(GameItem)) {
            equal = Equals((GameItem)other);
        }
        return equal;
    }

    public bool Equals(GameItem other) {
        return ((other.gameObject.name == gameObject.name) &&
                (other.GetType() == GetType()));
    }

    public override int GetHashCode() {
        return gameObject.name.GetHashCode() + GetType().GetHashCode();
    }

    public void Interact() {
        if (!Inventory.isItemSelected || !Inventory.SelectedItem.Equals(this)) {
            Interact(!Inventory.isItemSelected);
        }
    }

    public void Interact(bool examine) {
        InteractionTarget = this;
        Dialogue.SerializableTree dlg = dialogue;

        if (!examine) {
            dlg = Player.UseItemDialogue;
            Debug.Log("Using selected item with " + InteractionTarget);
        }
        else if (dlg == null) {
            dlg = Player.ExamineDialogue;
            Debug.Log("Examining " + InteractionTarget);
        }
        else {
            Debug.Log("Running attached dlg");
        }

        //DialogueManager.StartDialogue(dlg);
    }
}