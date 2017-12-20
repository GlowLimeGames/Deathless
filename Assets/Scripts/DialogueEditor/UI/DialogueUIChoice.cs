using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A dialogue choice the player can click on.
/// </summary>
public class DialogueUIChoice : MonoBehaviour {
    /// <summary>
    /// The dialogue node for this choice.
    /// </summary>
    public Dialogue.Node node;

    /// <summary>
    /// The LayoutElement component of this object.
    /// </summary>
    private LayoutElement layout;

    /// <summary>
    /// The Text component of this choice.
    /// </summary>
    private Text text;

    /// <summary>
    /// Initialize this choice UI to display the given node.
    /// </summary>
    public void Init(Dialogue.Node node) {
        this.node = node;
        layout = GetComponent<LayoutElement>();
        text = GetComponentInChildren<Text>();
        text.text = node.Data.Text;
    }

    void Update() {
        // Make sure the LayoutElement is sized to fit the text.
        if (layout.minHeight != text.rectTransform.sizeDelta.y) {
            layout.minHeight = text.rectTransform.sizeDelta.y;
        }
    }

    /// <summary>
    /// When clicked, show the dialogue that follows from this choice.
    /// Should be called from the Button component on the gameObject.
    /// </summary>
    public void OnClick() {
        if (UIManager.AllInputEnabled) { DialogueManager.Next(node); }
    }
}