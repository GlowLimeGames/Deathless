using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class DialogueManager : MonoBehaviour {

	public static void StartDialogue(DialogueTree dialogue) {

    }

    public static DialogueTree LoadDialogue(SerializableTree dialogue) {
        return dialogue.ImportTree();
    }
}