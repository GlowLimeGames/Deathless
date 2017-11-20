using UnityEngine;

public class MoveOnClick : MonoBehaviour {
    /// <summary>
    /// When the player clicks this object, their character
    /// should move toward the point they clicked.
    /// </summary>
    void OnMouseUpAsButton() {
        if (UIManager.WorldInputEnabled) {
            MovePlayerTowardPointer();
        }
    }

    private void MovePlayerTowardPointer() {
        GameItem.CancelInteraction();
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameManager.Player.MoveToPoint(pos);
    }
}