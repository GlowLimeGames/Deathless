using System.Collections;
using System.Collections.Generic;
using Dialogue;

public class DialogueTester {
	public static DialogueTree CreateTestTree() {
        DialogueTree tree = new DialogueTree();
        TestBranch(tree.root, 1);
        TestBranch(tree.root, 2);
        TestBranch(tree.root, 3);
        return tree;
    }

    private static void TestBranch(Node parent, float id) {
        Node node = TestNode(parent, id);
        parent.AddLink(node);
        TestNode(node, id + 0.1f);
        TestNode(node, id + 0.2f);
    }

    private static Node TestNode(Node parent, float id) {
        Node node = parent.AddNode(NodeType.CHOICE, "Choice " + id);
        return node.AddNode(NodeType.LINE, "Outcome " + id);
    }
}