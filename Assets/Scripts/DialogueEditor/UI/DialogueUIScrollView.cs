using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIScrollView : MonoBehaviour {
    private const float SCROLL_MULT = 10f;
    private float scrollRateNormalized;
    private bool scrollable;

    private ScrollRect scrollRect;

    #pragma warning disable 0649
    [SerializeField]
    private GameObject scrollUpButton, scrollDownButton, continueIcon;
    #pragma warning restore 0649

    void Awake() {
        scrollRect = gameObject.GetComponent<ScrollRect>();
    }

    void Update() {
        if (scrollable != scrollRect.verticalScrollbar.gameObject.activeInHierarchy) {
            ShowValidButtons();
        }
    } 

    public void InitializeNewContent(GameObject content, bool isChoice) {
        scrollRect.content = (RectTransform)content.transform;
        continueIcon.SetActive(!isChoice);

        StartCoroutine(LateInit());
    }

    private IEnumerator LateInit() {
        yield return new WaitForEndOfFrame();

        scrollRateNormalized = SCROLL_MULT * scrollRect.scrollSensitivity / scrollRect.content.rect.height;

        ResetScrollPos();
        ShowValidButtons();
    }

    private void ResetScrollPos() {
        SetScrollPos(1);
    }

	public void ScrollUp() {
        SetScrollPos(scrollRect.verticalNormalizedPosition + scrollRateNormalized);
    }

    public void ScrollDown() {
        SetScrollPos(scrollRect.verticalNormalizedPosition - scrollRateNormalized);
    }

    private void SetScrollPos(float pos) {
        scrollRect.verticalNormalizedPosition = Mathf.Clamp(pos, 0, 1);
    }

    public void ShowValidButtons() {
        scrollable = scrollRect.verticalScrollbar.gameObject.activeInHierarchy;

        bool showUp = (scrollable && scrollRect.verticalNormalizedPosition < 0.99f);
        bool showDown = (scrollable && scrollRect.verticalNormalizedPosition > 0.01f);

        scrollUpButton.gameObject.SetActive(showUp);
        scrollDownButton.gameObject.SetActive(showDown);
    }

    void OnDisable() {
        scrollUpButton.gameObject.SetActive(false);
        scrollDownButton.gameObject.SetActive(false);
    }
}