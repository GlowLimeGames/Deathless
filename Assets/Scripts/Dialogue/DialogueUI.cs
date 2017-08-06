using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;

/// <summary>
/// Handles how dialogue is displayed in-game.
/// </summary>
public class DialogueUI : Manager<DialogueUI> {
    private const string SPEECH_BUBBLE_TAG = "Speech bubble";

    /// <summary>
    /// Whether a dialogue is currently active.
    /// </summary>
    public static bool isShown {
        get { return instance != null && instance.gameObject.activeInHierarchy; }
    }

    /// <summary>
    /// The line of dialogue currently being shown.
    /// </summary>
    private Node currentNode = null;

    private WorldItem currentSpeaker;

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
    #pragma warning disable 0649
    private GameObject lineView, choiceView, choicePrefab;
    #pragma warning restore 0649

    /// <summary>
    /// The text shown for a single line of dialogue.
    /// </summary>
    private Text lineText;

    void Awake() {
        SingletonInit();

        lineText = lineView.GetComponentInChildren<Text>();
    }

    void Update() {
        // Advance dialogue when mouse is clicked.
        if (UIManager.AllInputEnabled && allowClick && Input.GetKeyUp(KeyCode.Mouse0)) {
            DialogueManager.Next(currentNode);
        }

        allowClick = (currentNode != null);
    }

    /// <summary>
    /// Show or hide the dialogue UI.
    /// </summary>
    public void Show(bool show) {
        gameObject.SetActive(show);
        UIManager.ShowCustomCursor(!show);
        UIManager.BlockWorldInput(show);
        UIManager.ShowHoverText(!show);

        if (!show) { Inventory.RevertSelection(); }
    }

    /// <summary>
    /// Show a single line of dialogue.
    /// </summary>
    public void ShowLine(Node line) {
        if (line.Data.Text != "" && line.Data.Text != null) {
            gameObject.SetActive(true);
            currentNode = line;
            lineText.text = line.Data.Text;
            lineView.SetActive(true);
            EnableSpeechBubble(line, false);
        }
        else {
            gameObject.SetActive(false);
            DialogueManager.Next(line);
        }
    }

    /// <summary>
    /// Show a set of choices.
    /// </summary>
    public void ShowChoices(List<Node> choices) {
        gameObject.SetActive(true);
        foreach (Node choice in choices) {
            if (choice.Data.Text != "") {
                DialogueUIChoice choiceUI = Instantiate(choicePrefab).GetComponent<DialogueUIChoice>();
                choiceUI.Init(choice);
                choiceUI.transform.SetParent(choiceView.transform, false);
            }
        }
        choiceView.SetActive(true);
        EnableSpeechBubble(choices[0], true);
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

        if (currentSpeaker != null) {
            currentSpeaker.ShowSpeechBubble(false);
            currentSpeaker = null;
        }
    }
    
    private void EnableSpeechBubble(Node node, bool isChoice) {
        if (node.Data.Speaker != null) {
            currentSpeaker = node.Data.Speaker.GetComponent<WorldItem>();
        }

        if (currentSpeaker == null) {
            if (!isChoice && DialogueManager.DlgInstance.Owner != null) {
                currentSpeaker = DialogueManager.DlgInstance.Owner.GetComponent<WorldItem>();
            }
            else { currentSpeaker = GameManager.Player; }
        }

        if (currentSpeaker != null) {
            currentSpeaker.ShowSpeechBubble(true);
        }
    }
}