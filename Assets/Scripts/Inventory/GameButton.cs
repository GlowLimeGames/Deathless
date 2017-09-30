using UnityEngine;
using UnityEngine.EventSystems;

public class GameButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler {
    [SerializeField]
    private string button_name;

    public void OnPointerEnter(PointerEventData eventData) {
        UIManager.BlockWorldInput(true);
        UIManager.SetHoverText(button_name);
    }

    public void OnPointerExit(PointerEventData eventData) {
        UIManager.BlockWorldInput(false);
        UIManager.ClearHoverText();
    }
}