using UnityEngine.EventSystems;

public abstract class Hoverable : SceneObject, IPointerEnterHandler, IPointerExitHandler {
    public abstract void OnHoverEnter();
    public abstract void OnHoverExit();

    public void OnPointerEnter(PointerEventData eventData) {
        OnHoverEnter();
    }

    public void OnPointerExit(PointerEventData eventData) {
        OnHoverExit();
    }

    void OnMouseEnter() { OnHoverEnter(); }
    void OnMouseExit() { OnHoverExit(); }
}