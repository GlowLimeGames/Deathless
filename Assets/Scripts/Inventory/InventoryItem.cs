using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour {
    public Dialogue.SerializableTree dialogue;

    public override bool Equals(object other) {
        bool equal = false;
        if (other.GetType() == typeof(InventoryItem)) {
            equal = Equals((InventoryItem)other);
        }
        return equal;
    }

    public bool Equals(InventoryItem other) {
        return (other.gameObject.name == this.gameObject.name);
    }

    public override int GetHashCode() {
        return gameObject.name.GetHashCode();
    }
}