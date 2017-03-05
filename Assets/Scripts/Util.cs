using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class Util {
    private static Texture2D CreateCursorTexture(Sprite sprite) {
        Texture2D newText = new Texture2D((int)sprite.textureRect.width, (int)sprite.textureRect.height, TextureFormat.ARGB32, false);
        Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                        (int)sprite.textureRect.y,
                                                        (int)sprite.textureRect.width,
                                                        (int)sprite.textureRect.height);
        newText.SetPixels(newColors);
        newText.Apply();
        return newText;
    }

    public static void SetCursor(Sprite sprite) {
        Texture2D texture = CreateCursorTexture(sprite);
        Vector2 hotspot = new Vector2(texture.width / 2, texture.height / 2);
        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
    }
}