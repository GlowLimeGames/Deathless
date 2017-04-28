﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    /// <summary>
    /// Allow a click to move dialogue forward. Necessary to create a
    /// single frame delay so clicks don't get registered in more than
    /// one place.
    /// </summary>
    private bool allowClick = false;

    /// <summary>
    /// Various GameObjects that are part of the dialogue UI.
    /// </summary>
    [SerializeField]
    private GameObject lineView, choiceView, choicePrefab;

    /// <summary>
    /// The text shown for a single line of dialogue.
    /// </summary>
    private Text lineText;

    void Start() {
        // Singleton
        if (instance == null) {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else { Destroy(this); }

        lineText = lineView.GetComponentInChildren<Text>();
    }

    void Update() {
        // Advance dialogue when mouse is clicked.
        if (allowClick && Input.GetKeyUp(KeyCode.Mouse0)) {
            DialogueManager.DisplayNext(currentNode);
        }

        allowClick = (currentNode != null);
    }

    /// <summary>
    /// Show or hide the dialogue UI.
    /// </summary>
    public void Show(bool show) {
        gameObject.SetActive(show);
        GameManager.InputEnabled = !show;
    }

    /// <summary>
    /// Show a single line of dialogue.
    /// </summary>
    public void ShowLine(Dialogue.Node line) {
        if (line.Data.Text != "") {
            currentNode = line;
            lineText.text = line.Data.Text;
            lineView.SetActive(true);
        }
        else {
            DialogueManager.DisplayNext(line);
        }
    }

    /// <summary>
    /// Show a set of choices.
    /// </summary>
    public void ShowChoices(List<Dialogue.Node> choices) {
        foreach (Dialogue.Node choice in choices) {
            if (choice.Data.Text != "") {
                DialogueUIChoice choiceUI = Instantiate(choicePrefab).GetComponent<DialogueUIChoice>();
                choiceUI.Init(choice);
                choiceUI.transform.SetParent(choiceView.transform, false);
            }
        }
        choiceView.SetActive(true);
    }

    /// <summary>
    /// Clear any dialogue lines or choices
    /// currently shown.
    /// </summary>
    public void ClearDialogue() {
        currentNode = null;
        foreach (DialogueUIChoice choice in choiceView.GetComponentsInChildren<DialogueUIChoice>()) {
            Destroy(choice.gameObject);
        }
        choiceView.SetActive(false);
        lineView.SetActive(false);
    }
}