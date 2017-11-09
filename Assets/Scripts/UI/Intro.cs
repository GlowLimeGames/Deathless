using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : Fadable {
    [SerializeField]
    private List<Fadable> sections = new List<Fadable>();

    [SerializeField]
    private float sectionDelay = 3f;

    private int sectionIndex = 0;
    private bool allowClick = false;
    
	void Start () {
        UIManager.FadeOut(0f);
        UIManager.ShowCustomCursor(false);
        UIManager.OnShowUIElement(true);

        foreach (Fadable section in sections) {
            section.gameObject.SetActive(false);
        }

        allowClick = false;
        StartFadeIn(DEFAULT_FADE_RATE, ShowNextSection);
	}

    public void OnButtonClick() {
        if (allowClick) {
            StopAllCoroutines();
            ShowNextSection();
        }
    }

    private void ShowNextSection() {
        if (sectionIndex < sections.Count) {
            allowClick = false;
            sections[sectionIndex].gameObject.SetActive(true);
            sections[sectionIndex].StartFadeIn(DEFAULT_FADE_RATE, SectionFadeComplete);
            sectionIndex++;
        }
    }

    private void SectionFadeComplete() {
        allowClick = true;
        StartCoroutine(DelaySection());
    }

    IEnumerator DelaySection() {
        yield return new WaitForSeconds(sectionDelay);
        ShowNextSection();
    }

    public void EndIntro() {
        StartFadeOut(DEFAULT_FADE_RATE, DisableIntro);
    }

    private void DisableIntro() {
        gameObject.SetActive(false);
        UIManager.ShowCustomCursor(true);
        UIManager.OnShowUIElement(false);
        UIManager.FadeIn();
    }
}