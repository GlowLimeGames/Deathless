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

    IEnumerator Fade (bool fadeIn, float fadeRate, FadeCallback fadeCallback) {
        Fadable[] children = GetFadableChildren();
        Color color = this.color;
        float alphaPercent = GetAlphaPercent(color.a);

        while ((fadeIn && alphaPercent < 1f) || (!fadeIn && alphaPercent > 0f)) {
            float delta = Time.deltaTime / fadeRate;
            alphaPercent += fadeIn ? delta : -delta;
            CopyAlphaToChildren(children, alphaPercent);

            color.a = GetAlphaValue(alphaPercent);
            this.color = color;
            yield return null;
        }

        if (fadeCallback != null) { fadeCallback(); }
    }

    private float GetAlphaValue(float alphaPercent) {
        return maxAlpha * alphaPercent;
    }

    private float GetAlphaPercent(float alphaValue) {
        return alphaValue / maxAlpha;
    }

    private Fadable[] GetFadableChildren() {
        List<GameObject> children = new List<GameObject>();

        foreach (Graphic childGraphic in GetComponentsInChildren<Graphic>()) {
            children.Add(childGraphic.gameObject);
        }
        foreach (SpriteRenderer childSprite in GetComponentsInChildren<SpriteRenderer>()) {
            children.Add(childSprite.gameObject);
        }

        foreach (GameObject child in children) {
            if (child.GetComponent<Fadable>() == null) {
                child.AddComponent<Fadable>();
            }
        }

        return GetComponentsInChildren<Fadable>();
    }

    private void CopyAlphaToChildren(Fadable[] children, float alphaPercent) {
        foreach (Fadable child in children) {
            Color color = child.color;
            color.a = child.GetAlphaValue(alphaPercent);
            child.color = color;
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

        return StartCoroutine(Fade(fadeIn, fadeRate, fadeCallback));
    }
}