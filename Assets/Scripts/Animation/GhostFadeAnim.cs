using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFadeAnim : MonoBehaviour {

	private float fadeRate;

	private float fadeDelay;

	[SerializeField]
	private float minFade = 2f;

	[SerializeField]
	private float maxFade = 5f;

    private bool fadeIn;
    private System.DateTime delayStart;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer SpriteRenderer {
        get {
            if (spriteRenderer == null) {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            return spriteRenderer;
        }
    }

    private bool dlgFade;

    void Start() {
        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
            delayStart = System.DateTime.Now;
			RandomizeFade ();
        }
    }

    void Update() {
        Color color = SpriteRenderer.color;

        if (delayStart != default(System.DateTime) &&
            (System.DateTime.Now - delayStart).TotalSeconds >= fadeDelay) {
            delayStart = default(System.DateTime);
        }

        if (fadeIn) {
            color.a += Time.deltaTime / fadeRate;
            if (color.a > 0.99f) {
				RandomizeFade ();
                if (dlgFade) { CompleteDialogueAction(); }
                delayStart = System.DateTime.Now;
                fadeIn = false;
            }
        }
        else if (delayStart == default(System.DateTime)) {
            color.a -= Time.deltaTime / fadeRate;
            if (color.a < 0.01f) {
                if (dlgFade) { CompleteDialogueAction(); }
                fadeIn = true;
            }
        }
        SpriteRenderer.color = color;
    }
		
	//randomizes fade time and fade delay for ghosts before each fade
	float RandomizeFade() {
		float flt = Random.Range (minFade, maxFade);
		fadeDelay = flt;
		fadeRate = flt;
		return flt;
	}

    public void StartFadeIn(bool isDialogueAction = false) { StartFade(0.01f, true, isDialogueAction); }

    public void StartFadeOut(bool isDialogueAction = false) { StartFade(0.99f, false, isDialogueAction); }

    private void StartFade(float startAlpha, bool fadeIn, bool isDialogueAction) {
        Color color = SpriteRenderer.color;
        color.a = startAlpha;
        SpriteRenderer.color = color;

        this.fadeIn = fadeIn;
        dlgFade = isDialogueAction;
        delayStart = default(System.DateTime);
    }

    private void CompleteDialogueAction() {
        Dialogue.Actions.CompletePendingAction();
        dlgFade = false;
    }
}