using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
    void OnMouseUpAsButton() {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameManager.Player.MoveToPoint(pos);
        }
    }
}
