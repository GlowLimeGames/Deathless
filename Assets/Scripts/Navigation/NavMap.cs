using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMap : MonoBehaviour {
    [SerializeField]
    private float minZDepth, maxZDepth;

    private Sprite sprite;
    private Bounds bounds;

    void Start() {
        sprite = GetComponent<SpriteRenderer>().sprite;
        bounds = GetComponent<Collider2D>().bounds;
    }

    public Vector2 GetNearestWalkablePoint (Vector2 target) {
        Vector2 point = target;

        if (!bounds.Contains(target)) {
            point = bounds.ClosestPoint(new Vector3 (target.x, target.y, this.transform.position.z));
        }

        return point;
    }

    public float GetZDepthAtScreenPoint (Vector2 point) {
        float grayscaleVal = GetColorAtPoint(point).grayscale;

        float diff = (maxZDepth - minZDepth);
        float depth = minZDepth + (diff * grayscaleVal);

        return depth;
    }

    private float GetRelativeDepthByColor (Color color) {
        return color.grayscale;
    }

    private Color GetColorAtPoint (Vector2 point) {
        //TEMP
        return Color.gray;
    }
}
