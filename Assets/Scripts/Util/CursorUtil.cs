using UnityEngine;

public static class CursorUtil {
    /// <summary>
    /// The size of custom cursors as a fraction of 
    /// screen height (1/CURSOR_SIZE).
    /// </summary>
    private const int CURSOR_SIZE = 20;

    private static Vector2 defaultCursorDimensions, cursorDimensions, cursorHotspot;

    public static void ConfineCustomCursor() {
        Rect cursor = new Rect(Input.mousePosition.x - cursorHotspot.x,
                                Input.mousePosition.y - cursorHotspot.y,
                                cursorDimensions.x,
                                cursorDimensions.y);

        if (Camera.main.pixelRect.Contains(cursor.min)
            && Camera.main.pixelRect.Contains(cursor.max)) {
            UIManager.ShowCustomCursor(true);
        }
        else { UIManager.ShowCustomCursor(false); }
    }

    public static void SetDefaultCursorDimensions(Texture2D defaultCursor) {
        defaultCursorDimensions = (defaultCursor == null) ? Vector2.zero : new Vector2(defaultCursor.width, defaultCursor.height);
        cursorDimensions = defaultCursorDimensions;
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
    private static Vector2 ResizeCursorSprite(Texture2D texture, int size) {
        float ratio = (float)texture.width / texture.height;

        int pixelHeight = Screen.height / size;
        int pixelWidth = Mathf.RoundToInt((float)pixelHeight * ratio);

        TextureScale.Bilinear(texture, pixelWidth, pixelHeight);
        return new Vector2(pixelWidth, pixelHeight);
    }

    /// <summary>
    /// Set the player's cursor to the given sprite.
    /// </summary>
	public static void SetCursor(Sprite sprite, bool hidden = false) {
        Texture2D texture = null;
        Vector2 hotspot;
        
        if (sprite == null) {
            if (!hidden) cursorDimensions = defaultCursorDimensions;
            hotspot = Vector2.zero;
        }
        else {
            texture = CreateCursorTexture(sprite);
            cursorDimensions = ResizeCursorSprite(texture, CURSOR_SIZE);
            hotspot = new Vector2(texture.width / 2, texture.height / 2);
        }

        if (!hidden) { cursorHotspot = hotspot; }
        Cursor.SetCursor(texture, cursorHotspot, CursorMode.ForceSoftware);
    }
}