using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZDepthMap : MonoBehaviour {
    #pragma warning disable 0649
    [SerializeField]
    private float minZDepth, maxZDepth;
    #pragma warning restore 0649

    private Sprite sprite;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>().sprite;
    }

    public float GetZDepthAtWorldPoint (Vector2 point) {
        float grayscaleVal = GetColorAtPoint(point).grayscale;

        float diff = (maxZDepth - minZDepth);
        float depth = minZDepth + (diff * grayscaleVal);

        return depth;
    }

    private Color GetColorAtPoint (Vector2 point) {
        Vector2 pos = transform.InverseTransformPoint(point);

        int x = Mathf.RoundToInt(pos.x * sprite.pixelsPerUnit);
        int y = Mathf.RoundToInt(pos.y * sprite.pixelsPerUnit);

        return sprite.texture.GetPixel(x, y);
    }
}