using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour {
    [SerializeField]
    private Dialogue.SerializableTree dialogue;

    private void OnTriggerEnter2D(Collider2D collision) {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null) {
            if (DialogueManager.StartDialogue(dialogue)) {
                player.StopMovement();
            }
        }
    }
}