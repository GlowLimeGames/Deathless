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

    public bool GetZDepthAtWorldPoint(Vector2 point, out float zPos) {
        Color color = GetColorAtPoint(point);
        if (color.a == 0) {
            zPos = 0;
            return false;
        }
        float grayscaleVal = GetColorAtPoint(point).grayscale;

        float diff = (maxZDepth - minZDepth);
        float depth = minZDepth + (diff * (1 - grayscaleVal));

        zPos = depth;
        return true;
    }

    private bool isPointOnZMap(Vector2 point) {
        return sprite.bounds.Contains(point);
    }

    public float GetZDepthAtWorldPoint (Vector3 position) {
        float zPos = position.z;

        if (isPointOnZMap(position)) {
            float grayscaleVal = GetColorAtPoint(position).grayscale;

            float diff = (maxZDepth - minZDepth);
            zPos = minZDepth + (diff * (1 - grayscaleVal));
        }

        return zPos;
    }

    private Color GetColorAtPoint (Vector2 point) {
        Vector2 pos = transform.InverseTransformPoint(point);

        int x = Mathf.RoundToInt(pos.x * sprite.pixelsPerUnit);
        int y = Mathf.RoundToInt(pos.y * sprite.pixelsPerUnit);

        return sprite.texture.GetPixel(x, y);
    }
}