using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to the background of a scene.
/// </summary>
public class World : MonoBehaviour {
    /// <summary>
    /// When the player clicks on the background, their character
    /// should move toward the point they clicked.
    /// </summary>
    void OnMouseUpAsButton() {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameManager.Player.MoveToPoint(pos);
        }
    }
}