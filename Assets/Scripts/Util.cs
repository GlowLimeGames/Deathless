using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains static utility functions.
/// </summary>
public static class Util {
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
    /// Set the player's cursor to the given sprite.
    /// </summary>
    public static void SetCursor(Sprite sprite) {
        Texture2D texture = CreateCursorTexture(sprite);
        Vector2 hotspot = new Vector2(texture.width / 2, texture.height / 2);
        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
    }

    /// <summary>
    /// Similar to GameObject.FindObjectsOfType but allows for
    /// the inclusion of inactive objects.
    /// </summary>
    public static T[] FindObjectsOfType<T>(bool includeInactive) where T : Component {
        List<T> results = new List<T>();
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject obj in rootObjects) {
            results.AddRange(obj.GetComponentsInChildren<T>(includeInactive));
        }
        
        return results.ToArray();
    }
}