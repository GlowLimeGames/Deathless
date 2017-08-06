using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles how dialogue is displayed in-game.
/// </summary>
public class DialogueUI : Manager<DialogueUI> {
    /// <summary>
    /// Whether a dialogue is currently active.
    /// </summary>
    public static bool isShown {
        get { return instance != null && instance.gameObject.activeInHierarchy; }
    }

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
    public void ShowLine(Dialogue.Node line) {
        if (line.Data.Text != "" && line.Data.Text != null) {
            currentNode = line;
            lineText.text = line.Data.Text;
            lineView.SetActive(true);
        }
        else {
            DialogueManager.Next(line);
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