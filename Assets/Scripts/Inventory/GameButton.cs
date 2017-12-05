using UnityEngine;

public class GameButton : Hoverable {
    [SerializeField]
    private string button_name;

    public override void OnHoverEnter() {
        CursorUtil.SetInteractionCursor(true);
        UIManager.SetHoverText(button_name);
        UIManager.BlockWorldInput(true);
    }

    public override void OnHoverExit() {
        CursorUtil.SetInteractionCursor(false);
        UIManager.ClearHoverText();
        UIManager.BlockWorldInput(false);
    }
}