using UnityEngine;

public class CameraAspect : MonoBehaviour {
    private const float ASPECT_RATIO = 16f / 9f;

    void Awake() { ScaleCamera(); }

    private void ScaleCamera() {
        float currentaspect = (float)Screen.width / Screen.height;
        float scaleheight = currentaspect / ASPECT_RATIO;

        Camera camera = GetComponent<Camera>();
        Rect newRect = camera.rect;

        if (scaleheight < 1.0f) {
            newRect.height *= scaleheight;
            newRect.y = (1.0f - scaleheight) / 2.0f;
        }
        else {
            float scalewidth = 1.0f / scaleheight;
            
            newRect.width *= scalewidth;
            newRect.x = (1.0f - scalewidth) / 2.0f;
        }

        camera.rect = newRect;
    }
}