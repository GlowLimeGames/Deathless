using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUI : MonoBehaviour {
    private DialogueUI instance;
    private Dialogue.Node currentNode = null;

    void Start() {
        if (instance == null) {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else { Destroy(this); }
    }

    void Update() {
        // Advance dialogue when mouse is clicked.
        if (currentNode != null && Input.GetKeyUp(KeyCode.Mouse0)) {
            DialogueManager.DisplayNext(currentNode);
        }
    }

    /// <summary>
    /// Show or hide the dialogue UI.
    /// </summary>
    public void Show(bool show) {
        gameObject.SetActive(show);
    }

    /// <summary>
    /// Show a single line of dialogue.
    /// </summary>
    public void ShowLine(Dialogue.Node line) {
        currentNode = line;
        //TBD
    }

    /// <summary>
    /// Show a set of choices.
    /// </summary>
    public void ShowChoices(List<Dialogue.Node> choices) {
        //TBD (Use DialogueUIChoice.cs)
    }

    /// <summary>
    /// Clear any dialogue lines or choices
    /// currently shown.
    /// </summary>
    public void ClearDialogue() {
        currentNode = null;
    }
}