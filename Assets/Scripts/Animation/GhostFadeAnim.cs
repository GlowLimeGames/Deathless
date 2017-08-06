using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFadeAnim : MonoBehaviour {
    [SerializeField]
    private float fadeRate = 2f;
    [SerializeField]
    private float fadeDelay = 2f;

    private bool fadeIn;
    private System.DateTime delayStart;

    private SpriteRenderer spriteRenderer;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        delayStart = System.DateTime.Now;
    }

    void Update() {Color color = spriteRenderer.color;
        if (delayStart != default(System.DateTime) &&
            (System.DateTime.Now - delayStart).TotalSeconds >= fadeDelay) {
            delayStart = default(System.DateTime);
        }

        if (fadeIn) {
            color.a += Time.deltaTime / fadeRate;
            if (color.a > 0.99f) {
                delayStart = System.DateTime.Now;
                fadeIn = false;
            }
        }
        else if (delayStart == default(System.DateTime)) {
            color.a -= Time.deltaTime / fadeRate;
            if (color.a < 0.01f) { fadeIn = true; }
        }
        spriteRenderer.color = color;
    }
}