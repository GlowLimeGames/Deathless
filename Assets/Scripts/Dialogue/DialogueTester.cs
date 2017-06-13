using System.Collections;
using System.Collections.Generic;
using Dialogue;

/// <summary>
/// Used to create dialogue trees for testing.
/// </summary>
public class DialogueTester {
    /// <summary>
    /// Create a test dialogue tree.
    /// </summary>
	public static DialogueTree CreateTestTree(UnityEngine.Transform treeObject) {
        DialogueTree tree = new DialogueTree(treeObject);
        TestBranch(tree.root, 1);
        TestBranch(tree.root, 2);
        TestBranch(tree.root, 3);
        return tree;
    }

    /// <summary>
    /// Add a choice with sub-choices as a child of the given node.
    /// </summary>
    private static void TestBranch(Node parent, float id) {
        Node[] segment = TestSegment(parent, id);
        TestSegment(segment[1], id + 0.2f);
    }

    /// <summary>
    /// Add a segment (a single choice and its child outcome) as a
    /// child of the given node. Returns the choice and outcome in
    /// an array.
    /// </summary>
    private static Node[] TestSegment(Node parent, float id) {
        Node choice = parent.AddNode(NodeType.CHOICE, "Choice " + id);
        Node outcome = choice.AddNode(NodeType.LINE, "Outcome " + id);

        return new Node[] { choice, outcome };
    }
}