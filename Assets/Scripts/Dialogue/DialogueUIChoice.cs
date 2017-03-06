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

    private LayoutElement layout;
    private Text text;

    public void Init(Dialogue.Node node) {
        this.node = node;
        layout = GetComponent<LayoutElement>();
        text = GetComponentInChildren<Text>();
        text.text = node.Data.Text;
    }

    void Update() {
        layout.minHeight = text.rectTransform.sizeDelta.y;
    }

    /// <summary>
    /// When clicked, show the dialogue that follows from this choice.
    /// Should be called from the Button component on the gameObject.
    /// </summary>
    public void OnClick() {
        DialogueManager.DisplayNext(node);
    }
}