using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUIChoice : MonoBehaviour {
    public Dialogue.Node node;

    void OnMouseUpAsButton() {
        DialogueManager.DisplayNext(node);
    }
}
