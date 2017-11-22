using System.Collections;
using UnityEngine;

public class GhostFadeAnim : Fadable {
    [SerializeField]
    private float minFade = 2f;
    [SerializeField]
    private float maxFade = 5f;

    private float fadeRate;
	private float fadeDelay;

    void Start() {
        if (currentlyRunningCoroutines.Count == 0) {
            BeginGhostFadeLoop();
        }
    }

    private void BeginGhostFadeLoop() {
        StartCoroutine(GhostFadeLoop());
    }

    private IEnumerator GhostFadeLoop() {
        while (true) {
            RandomizeFade();
            yield return new WaitForSeconds(fadeDelay);
            yield return StartFadeOut(fadeRate, null, false);
            yield return StartFadeIn(fadeRate, null, false);
        }
    }

    /// <summary>
    /// Randomizes fade time and fade delay for ghosts before each fade
    /// </summary>
    private float RandomizeFade() {
		float flt = Random.Range (minFade, maxFade);
		fadeDelay = flt;
		fadeRate = flt;
		return flt;
	}

    public void StartGhostFade(bool fadeIn, bool isDialogueAction = false) {
        FadeCallback callback = BeginGhostFadeLoop;
        if (isDialogueAction) { callback += Dialogue.Actions.CompletePendingAction; }

        if (fadeIn) { StartFadeIn(minFade, callback); }
        else { StartFadeOut(minFade, callback); }
    }
}