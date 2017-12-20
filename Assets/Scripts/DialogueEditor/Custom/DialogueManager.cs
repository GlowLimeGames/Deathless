/// <summary>
/// Manages dialogue trees in-game.
/// </summary>
public class DialogueManager : DialogueUIManager {
    protected override ISpeaker defaultSpeaker { get { return GameManager.Player; } }

    protected override bool AllowInput {
        get { return UIManager.AllInputEnabled; }
        set { UIManager.BlockAllInput(!value); }
    }

    public override void Show(bool show) {
        base.Show(show);
        CursorUtil.AllowCustomCursor = !show;
        UIManager.OnShowUIElement(show);

        if (!show) { Inventory.RevertSelection(); }
    }
}