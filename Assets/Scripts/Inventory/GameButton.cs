using UnityEngine;

public class GameButton : Hoverable {
    [SerializeField]
    private string button_name;

    public override void OnHoverEnter() {
        UIManager.SetHoverText(button_name);
        UIManager.BlockWorldInput(true);
    }

    public override void OnHoverExit() {
        UIManager.ClearHoverText();
        UIManager.BlockWorldInput(false);
    }
}