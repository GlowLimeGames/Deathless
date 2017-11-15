using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fadable : MonoBehaviour {
    public const float DEFAULT_FADE_RATE = 2f;

    public delegate void FadeCallback();
    protected List<Coroutine> currentlyRunningCoroutines = new List<Coroutine>();
    
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

    public new Coroutine StartCoroutine(IEnumerator coroutine) {
        return base.StartCoroutine(CoroutineWrapper(coroutine));
    }

    private IEnumerator CoroutineWrapper(IEnumerator coroutine) {
        Coroutine currentCoroutine = base.StartCoroutine(coroutine);
        currentlyRunningCoroutines.Add(currentCoroutine);
        
        yield return currentCoroutine;

        currentlyRunningCoroutines.Remove(currentCoroutine);
    }

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

    public void StopFade() {
        foreach (Coroutine coroutine in currentlyRunningCoroutines) {
            StopCoroutine(coroutine);
        }

        currentlyRunningCoroutines = new List<Coroutine>();
    }

    public virtual Coroutine StartFadeIn(float fadeRate = DEFAULT_FADE_RATE, FadeCallback fadeCallback = null, bool interruptCoroutines = true) {
        return StartFade(0f, true, fadeRate, fadeCallback, interruptCoroutines);
    }

    public virtual Coroutine StartFadeOut(float fadeRate = DEFAULT_FADE_RATE, FadeCallback fadeCallback = null, bool interruptCoroutines = true) {
        return StartFade(1f, false, fadeRate, fadeCallback, interruptCoroutines);
    }

    protected virtual Coroutine StartFade(float startAlpha, bool fadeIn, float fadeRate, FadeCallback fadeCallback, bool interruptCoroutines) {
        if (interruptCoroutines) { StopFade(); }

        Color color = this.color;
        color.a = Mathf.Min(startAlpha, maxAlpha);
        this.color = color;

        if (fadeIn) { return StartCoroutine(FadeIn(fadeRate, fadeCallback)); }
        else { return StartCoroutine(FadeOut(fadeRate, fadeCallback)); }
    }
}