using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

public class DialogueManager : MonoBehaviour {
    private static DialogueManager instance;

    [SerializeField]
    private DialogueUI ui;
    private static DialogueUI UI {
        get { return instance.ui; }
    }

    void Start() {
        if (instance == null) {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else { Destroy(this); }
    }

    public static void StartDialogue(SerializableTree dialogue) {
        StartDialogue(LoadDialogue(dialogue));
    }

	public static void StartDialogue(DialogueTree dialogue) {
        UI.Show(true);
        DisplayNext(dialogue.root);
    }

    public static DialogueTree LoadDialogue(SerializableTree dialogue) {
        return dialogue.ImportTree();
    }

    public static void DisplayNext(Node current) {
        UI.ClearDialogue();
        if (!isDialogueOver(current.Children)) {
            if (current.Children[0].Data.Type == NodeType.LINE) {
                DisplayNextLine(current);
            }
            else {
                DisplayNextChoice(current);
            }
        }
    }

    private static bool isDialogueOver(List<Node> nextNodes) {
        return isDialogueOver(nextNodes.ConvertAll(x => (BaseNode)x));
    }

    private static bool isDialogueOver(List<BaseNode> nextNodes) {
        if (nextNodes.Count < 1) {
            UI.Show(false);
            return true;
        }
        else { return false; }
    }

    private static void DisplayNextLine(Node current) {
        List<Node> lines = GetValidNodes(current.Children, false);
        if (!isDialogueOver(lines)) {
            UI.ShowLine(lines[0]);
        }
    }

    private static void DisplayNextChoice(Node current) {
        List<Node> choices = GetValidNodes(current.Children, true);
        if (!isDialogueOver(choices)) {
            UI.ShowChoices(choices);
        }
    }

    private static List<Node> GetValidNodes(List<BaseNode> nodes, bool getMultiple = true) {
        List<Node> validNodes = new List<Node>();
        for (int i = 0; i < nodes.Count && (getMultiple || validNodes.Count < 1); i++) {
            if (nodes[i].Data.Condition.isValid) {
                validNodes.Add(nodes[i].GetOriginal());
            }
        }
        return validNodes;
    }
}