using UnityEngine;

public class HoverText : MonoBehaviour {
    private RectTransform rectTransform;
    private Vector2 defaultPivot;

    void Start() {
        rectTransform = (RectTransform)transform;
        defaultPivot = rectTransform.pivot;
    }

	void Update () {
        CalculatePosition();
    }

    private void CalculatePosition() {
        Vector2 screenPos = CursorUtil.OffsetFromCursor();

        Rect rect = GetDefaultScreenRect(rectTransform.rect, screenPos);
        Vector2 pivot = rectTransform.pivot;

        if (rect.yMin < CameraAspect.Bounds.yMin) {
            pivot.y = 0;
            screenPos = CursorUtil.OffsetFromCursor(false);
        }
        else if (pivot.y < 1) {
            pivot.y = 1;
        }
        if (rect.xMin < CameraAspect.Bounds.xMin) {
            screenPos.x = CameraAspect.Bounds.xMin + (rect.width * pivot.x);
        }
        else if (rect.xMax > CameraAspect.Bounds.xMax) {
            screenPos.x = CameraAspect.Bounds.xMax - (rect.width * (1 - pivot.x));
        }
        
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        rectTransform.pivot = pivot;
    }

    private Rect GetDefaultScreenRect(Rect rect, Vector2 screenPos) {
        screenPos.x -= rect.width * defaultPivot.x;
        screenPos.y -= rect.height * defaultPivot.y;

        return new Rect(screenPos.x, screenPos.y, rect.width, rect.height);
    }
}