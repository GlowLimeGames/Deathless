using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles how dialogue is displayed in-game.
/// </summary>
public class DialogueUI : MonoBehaviour {
    /// <summary>
    /// The instance of the DialogueUI in the current scene.
    /// </summary>
    private DialogueUI instance;

    /// <summary>
    /// The line of dialogue currently being shown.
    /// </summary>
    private Dialogue.Node currentNode = null;

    void Start() {
        // Singleton
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