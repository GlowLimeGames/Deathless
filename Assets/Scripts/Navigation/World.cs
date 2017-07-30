using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum CardinalDirection { NORTH = 0, SOUTH = 1, EAST = 2, WEST = 3 }

/// <summary>
/// Attached to the background of a scene.
/// </summary>
public class World : MonoBehaviour {
    /// <summary>
    /// When the player clicks on the background, their character
    /// should move toward the point they clicked.
    /// </summary>
    void OnMouseUpAsButton() {
        if (UIManager.WorldInputEnabled) {
            GameItem.CancelInteraction();
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameManager.Player.MoveToPoint(pos);
        }
    }

    public static void UpdateNavGraph() {
        AstarPath.active.Scan();
    }

    public static void UpdateNavGraph(GameObject obj) {
        Collider2D coll = obj.GetComponent<Collider2D>();
        if (coll != null) {
            GraphUpdateObject graphUpdate = new GraphUpdateObject(coll.bounds);
            graphUpdate.updatePhysics = true;
            AstarPath.active.UpdateGraphs(graphUpdate);
        }
    }
}