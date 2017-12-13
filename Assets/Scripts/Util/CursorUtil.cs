using UnityEngine;

public static class CursorUtil {
    /// <summary>
    /// The size of custom cursors as a fraction of 
    /// screen height (1/CURSOR_SIZE).
    /// </summary>
    private const int CURSOR_SIZE = 18;

    private static Vector2 cursorDimensions, cursorHotspot, currentHotspot;

    private static Sprite cursorSprite;
    private static Texture2D cursorTex;

    private static bool allowCustomCursor = true;
    public static bool AllowCustomCursor {
        get { return allowCustomCursor; }
        set {
            allowCustomCursor = value;
            ShowCustomCursor(value);
        }
    }

    public static void ConfineCustomCursor() {
        if (cursorSprite != null && cursorDimensions != Vector2.zero) {
            Rect cursor = new Rect(Input.mousePosition.x - cursorHotspot.x,
                                    Input.mousePosition.y - (cursorDimensions.y - cursorHotspot.y),
                                    cursorDimensions.x,
                                    cursorDimensions.y);

            // Yayyyyy logic
            bool containsMinX = cursor.xMin >= CameraAspect.Bounds.xMin,
                 containsMinY = cursor.yMin >= CameraAspect.Bounds.yMin,
                 containsMaxX = cursor.xMax <= CameraAspect.Bounds.xMax,
                 containsMaxY = cursor.yMax <= CameraAspect.Bounds.yMax,
                 containsAny = containsMinX || containsMinY || containsMaxX || containsMaxY,
                 containsAll = containsMinX && containsMinY && containsMaxX && containsMaxY,
                 containsPointer = Camera.main.pixelRect.Contains(Input.mousePosition);
            
            if (containsPointer && containsAny) {
                if (containsAll) { currentHotspot = cursorHotspot; }
                else {
                    if (!containsMaxX) {
                        currentHotspot.x = cursorDimensions.x - (CameraAspect.Bounds.xMax - Input.mousePosition.x);
                    }
                    else if (!containsMinX) {
                        currentHotspot.x = Input.mousePosition.x - CameraAspect.Bounds.xMin;
                    }

                    if (!containsMaxY) {
                        currentHotspot.y = CameraAspect.Bounds.yMax - Input.mousePosition.y;
                    }
                    else if (!containsMinY) {
                        currentHotspot.y = cursorDimensions.y - (Input.mousePosition.y - CameraAspect.Bounds.yMin);
                    }
                }
                ShowCustomCursor(true);
            }
            else { ShowCustomCursor(false); }
        }
    }

    /// <summary>
    /// Convert a sprite to a Texture2D that is valid for use as a cursor.
    /// </summary>\
    private static Texture2D CreateCursorTexture(Sprite sprite) {
        Texture2D newText = new Texture2D((int)sprite.textureRect.width, (int)sprite.textureRect.height, TextureFormat.ARGB32, false);
        Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                        (int)sprite.textureRect.y,
                                                        (int)sprite.textureRect.width,
                                                        (int)sprite.textureRect.height);
        newText.SetPixels(newColors);
        newText.Apply();

        #if UNITY_EDITOR
        newText.alphaIsTransparency = true;
        #endif

        return newText;
    }

    /// <summary>
    /// Takes a texture, changes its height and width to a certain fraction of screen height (1/size).
    /// </summary>
    private static Vector2 ResizeCursorSprite(Texture2D texture, float size) {
        float ratio = (float)texture.width / texture.height;

        float height = Screen.height / size;
        float width = height;

        if (ratio > 1) { height = width / ratio; }
        else { width = height * ratio; }

        int pixelHeight = Mathf.CeilToInt(height);
        int pixelWidth = Mathf.CeilToInt(width);

        TextureScale.Bilinear(texture, pixelWidth, pixelHeight);
        return new Vector2(pixelWidth, pixelHeight);
    }

    /// <summary>
    /// Set the player's cursor to the given sprite.
    /// </summary>
	private static void SetCursor(Sprite sprite) {
        CursorMode mode;

        if (sprite == null) {
            cursorTex = null;
            cursorDimensions = Vector2.zero;
            cursorHotspot = Vector2.zero;
            mode = CursorMode.Auto;
        }
        else {
            cursorTex = CreateCursorTexture(sprite);
            cursorDimensions = ResizeCursorSprite(cursorTex, CURSOR_SIZE);
            mode = CursorMode.ForceSoftware;
        }

        if (sprite != null && sprite != UIManager.InteractIcon) {
            cursorHotspot = new Vector2(cursorTex.width / 2, cursorTex.height / 2);
        }
        else { cursorHotspot = Vector2.zero; }

        currentHotspot = cursorHotspot;
        Cursor.SetCursor(cursorTex, currentHotspot, mode);
    }
    
    public static void SetCustomCursor(Sprite cursor) {
        cursorSprite = cursor;
        SetCursor(cursorSprite);
    }

    public static void SetInteractionCursor(bool show) {
        if (show && cursorSprite == null) {
            SetCustomCursor(UIManager.InteractIcon);
        }
        else if (!show && cursorSprite == UIManager.InteractIcon) {
            SetCustomCursor(null);
        }
    }

    private static void ShowCustomCursor(bool show) {
        if (show && cursorTex != null && AllowCustomCursor) {
            Cursor.SetCursor(cursorTex, currentHotspot, CursorMode.ForceSoftware);
        }
        else {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    public static Vector2 OffsetFromCursor(bool below = true) {
        Vector2 pos = Input.mousePosition;

        pos.x += (cursorDimensions.x / 2) - cursorHotspot.x;
        pos.y += below ? -(cursorDimensions.y - cursorHotspot.y) : cursorHotspot.y;

        return pos;
    }
}