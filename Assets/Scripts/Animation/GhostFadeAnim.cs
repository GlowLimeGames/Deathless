using System.Collections;
using UnityEngine;

public class GhostFadeAnim : Fadable {
    [SerializeField]
    private float minFade = 2f;
    [SerializeField]
    private float maxFade = 5f;

    private float fadeRate;
	private float fadeDelay;
    private bool fadeIn;
    private bool dlgFade;

    void Start() {
        RandomizeFade();
        StartCoroutine(DelayFade());
    }

    private void Fade() {
        RandomizeFade();
        if (fadeIn) { StartFadeIn(fadeRate, FadeComplete); }
        else { StartFadeOut(fadeRate, FadeComplete); }
    }

    private void FadeComplete() {
        fadeIn = !fadeIn;

        if (fadeIn) { Fade(); }
        else { StartCoroutine(DelayFade()); }

        if (dlgFade) { CompleteDialogueAction(); }
    }

    public override void StopFade() {
        StopAllCoroutines();
    }

    IEnumerator DelayFade() {
        yield return new WaitForSeconds(fadeDelay);
        Fade();
    }

    /// <summary>
    /// Randomizes fade time and fade delay for ghosts before each fade
    /// </summary>
    float RandomizeFade() {
		float flt = Random.Range (minFade, maxFade);
		fadeDelay = flt;
		fadeRate = flt;
		return flt;
	}

    public void StartGhostFade(bool fadeIn, bool isDialogueAction = false) {
        this.fadeIn = fadeIn;
        dlgFade = isDialogueAction;
        Fade();
    }

    private void CompleteDialogueAction() {
        Dialogue.Actions.CompletePendingAction();
        dlgFade = false;
    }
}