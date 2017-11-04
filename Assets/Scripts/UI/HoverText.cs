using UnityEngine;

public class HoverText : MonoBehaviour {
    private const float PIXEL_BUFFER = 5;
    private Rect bounds;
    private RectTransform rectTransform;
    private Vector2 defaultPivot;

    void Start() {
        rectTransform = (RectTransform)transform;
        defaultPivot = rectTransform.pivot;
        bounds = GetBounds();
    }

	void Update () {
        CalculatePosition();
    }

    private void CalculatePosition() {
        Vector2 screenPos = Input.mousePosition;
        screenPos.y -= PIXEL_BUFFER;

        Rect rect = GetDefaultScreenRect(rectTransform.rect, screenPos);
        Vector2 pivot = rectTransform.pivot;

        if (rect.xMin < bounds.xMin) {
            screenPos.x = bounds.xMin + (rect.width * pivot.x);
        }
        else if (rect.xMax > bounds.xMax) {
            screenPos.x = bounds.xMax - (rect.width * (1 - pivot.x));
        }
        if (rect.yMin < bounds.yMin) {
            pivot.y = 0;
            screenPos.y += PIXEL_BUFFER * 2;
        }
        else if (pivot.y < 1) {
            pivot.y = 1;
        }
        
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        rectTransform.pivot = pivot;
    }

    private Rect GetBounds() {
        Rect bounds = Camera.main.pixelRect;
        bounds.x += PIXEL_BUFFER;
        bounds.width -= PIXEL_BUFFER * 2;
        return bounds;
    }

    private Rect GetDefaultScreenRect(Rect rect, Vector2 screenPos) {
        screenPos.x -= rect.width * defaultPivot.x;
        screenPos.y -= rect.height * defaultPivot.y;

        return new Rect(screenPos.x, screenPos.y, rect.width, rect.height);
    }
}