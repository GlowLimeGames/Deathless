using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A dialogue choice the player can click on.
/// </summary>
public class DialogueUIChoice : MonoBehaviour, IPointerClickHandler {
    /// <summary>
    /// The dialogue node for this choice.
    /// </summary>
    public Dialogue.Node node;

    /// <summary>
    /// When clicked, show the dialogue that follows from this choice.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData) {
        DialogueManager.DisplayNext(node);
    }
}