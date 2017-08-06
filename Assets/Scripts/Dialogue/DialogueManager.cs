using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;

/// <summary>
/// Manages dialogue trees in-game.
/// </summary>
public class DialogueManager : Manager<DialogueManager> {
    /// <summary>
    /// Backing field for UI.
    /// </summary>
    [SerializeField]
    private DialogueUI ui;

    /// <summary>
    /// The DialogueUI object in the current scene.
    /// </summary>
    private static DialogueUI UI {
        get { return instance.ui; }
    }

    private SerializableTree dlgInstance;
    public static SerializableTree DlgInstance {
        get { return instance.dlgInstance; }
        private set { instance.dlgInstance = value; }
    }
    
    private static bool redirected;

    /// <summary>
    /// Begin the given dialogue tree.
    /// </summary>
    public static bool StartDialogue(SerializableTree dialogue) {
        return StartDialogue(LoadDialogue(dialogue));
    }

    public static void RedirectDialogue(SerializableTree dialogue) {
        UI.ClearDialogue();
        if (DlgInstance != null) { DlgInstance.CleanupTempInstance(); }
        StartDialogue(dialogue);
        redirected = true;
    }

    /// <summary>
    /// Begin the given dialogue tree.
    /// </summary>
	private static bool StartDialogue(DialogueTree dialogue) {
        UI.Show(true);
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
        if (current.Data.Actions != null) {
            UIManager.BlockAllInput(true);
            current.Data.Actions.Invoke(current);
            return true;
        }
        else { return DisplayNext(current); }
    }

    public static bool Continue(Node current) {
        UIManager.BlockAllInput(false);
        return DisplayNext(current);
    }

    private static bool DisplayNext(Node current) {
        if (!redirected) {
            UI.ClearDialogue();
            if (!isDialogueOver(current.Children)) {
                if (current.Children[0].Data.Type == NodeType.LINE) {
                    DisplayNextLine(current);
                }
                else {
                    DisplayNextChoice(current);
                }
                return true;
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
        UI.Show(false);
        if (DlgInstance != null) {
            DlgInstance.CleanupTempInstance();
        }
    }

    /// <summary>
    /// Display the first valid dialogue node that comes after the given one.
    /// </summary>
    private static void DisplayNextLine(Node current) {
        List<Node> lines = GetValidNodes(current.Children, false);
        if (!isDialogueOver(lines)) {
            UI.ShowLine(lines[0]);
        }
    }

    /// <summary>
    /// Display all valid dialogue nodes that come after the given one, as
    /// selectable choices.
    /// </summary>
    /// <param name="current"></param>
    private static void DisplayNextChoice(Node current) {
        List<Node> choices = GetValidNodes(current.Children, true);
        if (!isDialogueOver(choices)) {
            UI.ShowChoices(choices);
        }
    }
    
    /// <summary>
    /// Return the nodes from the given list that are currently valid.
    /// </summary>
    /// <param name="getMultiple">If this is false, only return the first valid node.</param>
    private static List<Node> GetValidNodes(List<BaseNode> nodes, bool getMultiple = true) {
        List<Node> validNodes = new List<Node>();
        for (int i = 0; i < nodes.Count && (getMultiple || validNodes.Count < 1); i++) {
            if (nodes[i].Data.Conditions == null || nodes[i].Data.Conditions.isValid) {
                validNodes.Add(nodes[i].GetOriginal());
            }
        }
        return validNodes;
    }
}