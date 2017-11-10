using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fadable : MonoBehaviour {
    public const float DEFAULT_FADE_RATE = 2f;

    public delegate void FadeCallback();
    
    private SpriteRenderer spriteRenderer;
    private Graphic graphic;

    private float maxAlpha;

    protected Color color {
        get {
            if (graphic != null) { return graphic.color; }
            else { return spriteRenderer.color; }
        }
        set {
            if (graphic != null) { graphic.color = value; }
            else { spriteRenderer.color = value; }
        }
    }

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        graphic = GetComponent<Graphic>();

        maxAlpha = color.a;
    }

    void OnDestroy() { StopFade(); }

    IEnumerator FadeIn (float fadeRate, FadeCallback fadeCallback) {
        Color color = this.color;

        while (color.a < 1f) {
            color.a += Time.deltaTime / fadeRate;
            CopyAlphaToChildren(color.a);

            Color maxColor = color;
            maxColor.a = Mathf.Min(maxColor.a, maxAlpha);
            this.color = maxColor;

            yield return null;
        }

        if (fadeCallback != null) { fadeCallback(); }
    }

    IEnumerator FadeOut(float fadeRate, FadeCallback fadeCallback) {
        Color color = this.color;

        while (color.a > 0f) {
            color.a -= Time.deltaTime / fadeRate;
            CopyAlphaToChildren(color.a);
            this.color = color;
            yield return null;
        }

        if (fadeCallback != null) { fadeCallback(); }
    }

    private void CopyAlphaToChildren(float alpha) {
        foreach (Graphic childGraphic in GetComponentsInChildren<Graphic>()) {
            Color color = childGraphic.color;
            color.a = alpha;
            childGraphic.color = color;
        }

        foreach (SpriteRenderer childSprite in GetComponentsInChildren<SpriteRenderer>()) {
            Color color = childSprite.color;
            color.a = alpha;
            childSprite.color = color;
        }
    }

    public virtual void StopFade() {
        StopAllCoroutines();
    }

    public virtual void StartFadeIn(float fadeRate = DEFAULT_FADE_RATE, FadeCallback fadeCallback = null) {
        StartFade(0f, true, fadeRate, fadeCallback);
    }

    public virtual void StartFadeOut(float fadeRate = DEFAULT_FADE_RATE, FadeCallback fadeCallback = null) {
        StartFade(1f, false, fadeRate, fadeCallback);
    }

    protected virtual void StartFade(float startAlpha, bool fadeIn, float fadeRate, FadeCallback fadeCallback) {
        StopFade();

        Color color = this.color;
        color.a = Mathf.Min(startAlpha, maxAlpha);
        this.color = color;

        if (fadeIn) { StartCoroutine(FadeIn(fadeRate, fadeCallback)); }
        else { StartCoroutine(FadeOut(fadeRate, fadeCallback)); }
    }
}