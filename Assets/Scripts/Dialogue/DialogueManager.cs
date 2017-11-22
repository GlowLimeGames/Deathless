using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;

/// <summary>
/// Manages dialogue trees in-game.
/// </summary>
public class DialogueManager : Manager<DialogueManager> {
    private const string SPEECH_BUBBLE_TAG = "Speech bubble";

	private const float CLICK_DELAY = 0.3f;

    private SerializableTree dlgInstance;
    public static SerializableTree DlgInstance {
        get { return Instance.dlgInstance; }
        private set { Instance.dlgInstance = value; }
    }

    /// <summary>
    /// Whether a dialogue is currently active.
    /// </summary>
    public static bool isShown {
        get { return Instance != null && Instance.gameObject.activeInHierarchy; }
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

    [SerializeField]
    private DialogueUIScrollView scrollView;

    /// <summary>
    /// The text shown for a single line of dialogue.
    /// </summary>
    private Text lineText;
    
    private static bool redirected;
    private static List<int> dontRepeat = new List<int>();

    public void Init() {
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

    public static void ResetOneShotDialogue() {
        dontRepeat = new List<int>();
    }

    /// <summary>
    /// Begin the given dialogue tree.
    /// </summary>
    public static bool StartDialogue(SerializableTree dialogue) {
        return StartDialogue(LoadDialogue(dialogue));
    }

    public static void RedirectDialogue(SerializableTree dialogue) {
        Instance.ClearDialogue();
        if (DlgInstance != null) { DlgInstance.CleanupTempInstance(); }
        StartDialogue(dialogue);
        redirected = true;
    }

    /// <summary>
    /// Begin the given dialogue tree.
    /// </summary>
	private static bool StartDialogue(DialogueTree dialogue) {
        Show(true);
        return Next(dialogue.root);
    }

    /// <summary>
    /// Load a DialogueTree from the given SerializableTree.
    /// </summary>
    public static DialogueTree LoadDialogue(SerializableTree dialogue) {
        SerializableTree dlg;
        if (dialogue.TryInstantiateTree(out dlg)) {
            DlgInstance = dlg;
        }

        return dlg.ImportTree();
    }

    /// <summary>
    /// Display the dialogue node(s) that come after the given one.
    /// </summary>
    public static bool Next(Node current) {
        if (current.Data.Restriction != RepeatRestriction.NONE) {
            if (current.Data.Restriction == RepeatRestriction.ONCE_PER_GAME) {
                dontRepeat.Add(current.Data.ID);
            }
            current.Data.Restriction = RepeatRestriction.DONT_SHOW;
        }
        if (current.Data.Actions != null) {
            UIManager.BlockAllInput(true);
            return current.Data.Actions.Invoke(current);
        }
        else { return DisplayNext(current); }
    }

    public static bool Continue(Node current) {
        UIManager.BlockAllInput(false);
        return DisplayNext(current);
    }

    private static bool DisplayNext(Node current) {
        if (!redirected) {
            Instance.ClearDialogue();
            if (!isDialogueOver(current.Children)) {
                if (current.Children == null) { Debug.Log("children is null"); }
                else if (current.Children[0] == null) { Debug.Log("child 0 is null"); }
                else if (current.Children[0].Data == null) { Debug.Log("child 0 data is null"); }
                if (current.Children[0].Data.Type == NodeType.LINE) {
                    return DisplayNextLine(current);
                }
                else {
                    return DisplayNextChoice(current);
                }
            }
            else { return false; }
        }
        else {
            redirected = false;
            return false;
        }
    }

    /// <summary>
    /// Return true if we have reached the end of the dialogue.
    /// </summary>
    private static bool isDialogueOver(List<Node> nextNodes) {
        return isDialogueOver(nextNodes.ConvertAll(x => (BaseNode)x));
    }

    /// <summary>
    /// Return true if we have reached the end of the dialogue.
    /// </summary>
    private static bool isDialogueOver(List<BaseNode> nextNodes) {
        if (nextNodes.Count < 1) {
            EndDialogue();
            return true;
        }
        else { return false; }
    }

    private static void EndDialogue () {
        Show(false);
        if (DlgInstance != null) {
            DlgInstance.CleanupTempInstance();
        }
    }

    /// <summary>
    /// Display the first valid dialogue node that comes after the given one.
    /// </summary>
    private static bool DisplayNextLine(Node current) {
        List<Node> lines = GetValidNodes(current.Children, current.Data.RandomizeChildren);
        if (!isDialogueOver(lines)) {
            if (current.Data.RandomizeChildren) {
                Instance.ShowLine(lines[Random.Range(0, lines.Count)]);
            }
            else { Instance.ShowLine(lines[0]); }
            return true;
        }
        else { return false; }
    }

    /// <summary>
    /// Display all valid dialogue nodes that come after the given one, as
    /// selectable choices.
    /// </summary>
    /// <param name="current"></param>
    private static bool DisplayNextChoice(Node current) {
        List<Node> choices = GetValidNodes(current.Children, true);
        if (!isDialogueOver(choices)) {
            Instance.ShowChoices(choices);
            return true;
        }
        else { return false; }
    }
    
    /// <summary>
    /// Return the nodes from the given list that are currently valid.
    /// </summary>
    /// <param name="getMultiple">If this is false, only return the first valid node.</param>
    private static List<Node> GetValidNodes(List<BaseNode> nodes, bool getMultiple = true) {
        List<Node> validNodes = new List<Node>();
        for (int i = 0; i < nodes.Count && (getMultiple || validNodes.Count < 1); i++) {
            if (isValidNode(nodes[i])) {
                validNodes.Add(nodes[i].GetOriginal());
            }
        }
        return validNodes;
    }

    private static bool isValidNode(BaseNode node) {
        bool valid = false;

        if (node.Data.Restriction != RepeatRestriction.DONT_SHOW) {
            if (node.Data.Restriction == RepeatRestriction.ONCE_PER_GAME && dontRepeat.Contains(node.Data.ID)) {
                node.Data.Restriction = RepeatRestriction.DONT_SHOW;
            }
            else if (node.Data.Conditions == null || node.Data.Conditions.isValid) {
                valid = true;
            }
        }

        return valid;
    }

    /// <summary>
    /// Show or hide the dialogue UI.
    /// </summary>
    public static void Show(bool show) {
        Instance.gameObject.SetActive(show);
        UIManager.ShowCustomCursor(!show);
        UIManager.OnShowUIElement(show);

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
            scrollView.InitializeNewContent(lineView, false);
            EnableSpeechBubble(line, false);
			StartCoroutine (DelayClick ());
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
        scrollView.InitializeNewContent(choiceView, true);
        EnableSpeechBubble(choices[0], true);
		StartCoroutine (DelayClick ());
    }

	/// <summary>
	/// Delays the ability to click for clickDelay seconds.
	/// </summary>
	IEnumerator DelayClick() {
		UIManager.BlockAllInput (true);
		yield return new WaitForSeconds (CLICK_DELAY);
		UIManager.BlockAllInput (false);
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
            if (!isChoice && DlgInstance.Owner != null) {
                currentSpeaker = DlgInstance.Owner.GetComponent<WorldItem>();
            }
            else { currentSpeaker = GameManager.Player; }
        }

        if (currentSpeaker != null) {
            currentSpeaker.ShowSpeechBubble(true);
        }
    }
}