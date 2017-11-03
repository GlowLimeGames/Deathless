﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : Manager<UIManager> {
    /// <summary>
    /// The size of custom cursors as a fraction of 
    /// screen height (1/CURSOR_SIZE).
    /// </summary>
    private const int CURSOR_SIZE = 20;

    /// <summary>
    /// The Inventory UI object in the current scene.
    /// </summary>
    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private DialogueManager dialogueManager;

    [SerializeField]
    private GameObject[] gameButtons;

    [SerializeField]
    private Text hoverText;

    [SerializeField]
    private Image blackout;

    [SerializeField]
    private Sprite interactIcon;

    [SerializeField]
    private AnimController genericAnimPrefab;
    public static AnimController GenericAnimPrefab { get { return Instance.genericAnimPrefab; } }
    
    [SerializeField]
    private float fadeTime = 2f;
    public static float FadeTime { get { return Instance.fadeTime; } }

    private System.DateTime fadeStart;
    private float currentFadeTime;
    private bool dlgActionFade;

    private static Sprite cursorIcon;

    private bool catchButtonPress;

    private bool worldInputEnabled;

    /// <summary>
    /// Whether world input (clicking items in the world such
    /// as characters, object, and the background) is enabled.
    /// </summary>
    public static bool WorldInputEnabled {
        get { return Instance.worldInputEnabled && AllInputEnabled; }
        private set { Instance.worldInputEnabled = value; }
    }

    private bool allInputEnabled;

    /// <summary>
    /// Whether player input of any kind is enabled.
    /// </summary>
    public static bool AllInputEnabled {
        get { return (Instance.allInputEnabled); }
        private set { Instance.allInputEnabled = value; }
    }

    void Start() {
        SingletonInit();

        AllInputEnabled = true;
        WorldInputEnabled = true;
        ClearHoverText();
        if (inventory != null) { inventory.Init(); }
        if (dialogueManager != null) { dialogueManager.Init(); }
    }

    void Update() {
        if (AllInputEnabled) {
            // Handle generic input
            if (inventory != null && Input.GetMouseButtonUp(1)) {
                if (Inventory.SelectedItem == null && !DialogueManager.isShown) {
                    Inventory.Show(!Inventory.isShown);
                }
                else {
                    Inventory.ClearSelection();
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape) && GameManager.GetCurrentScene() != GameManager.MAIN_MENU_SCENE) {
            GameManager.LoadScene(GameManager.MAIN_MENU_SCENE);
        }

        if (blackout != null && blackout.gameObject.activeInHierarchy &&
            (System.DateTime.Now - fadeStart).TotalSeconds >= currentFadeTime) {
            blackout.gameObject.SetActive(false);
            if (dlgActionFade) { Dialogue.Actions.CompletePendingAction(); }
        }
    }
    
    public static void OnShowUIElement(bool show) {
        BlockWorldInput(show);
        ShowGameButtons(!show);
        UpdateCursorHover();
    }

    private static void UpdateCursorHover() {
        SetInteractionCursor(false);
        ClearHoverText();

        Collider2D hitColl = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Hoverable hitItem = null;

        if (hitColl != null) {
            hitItem = hitColl.GetComponent<GameItem>();
        }

        if (hitItem != null) {
            hitItem.OnHoverEnter();
        }
    }

    public static void SetCustomCursor(Sprite cursor) {
        cursorIcon = cursor;
        ShowCustomCursor(true);
    }

    public static void SetInteractionCursor(bool show) {
        if (show) {
            if (cursorIcon == null) { SetCustomCursor(Instance.interactIcon); }
        }
        else if (cursorIcon == Instance.interactIcon) { ClearCustomCursor(); }
    }

    public static void ClearCustomCursor() {
        cursorIcon = null;
        ShowCustomCursor(false);
    }
    
	public static void ShowCustomCursor(bool show) {
		if (show && cursorIcon != null) {
			Util.SetCursor(cursorIcon, CURSOR_SIZE);
		} else { 
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); 
		}
	}

    public static void BlockWorldInput(bool block) {
        if (!block && !Inventory.isShown && !DialogueManager.isShown) {
            WorldInputEnabled = (true);
        }
        else { WorldInputEnabled = (false); }
    }

    public static void BlockAllInput(bool block) {
        AllInputEnabled = !block;
    }

    /// <summary>
    /// Load a different game scene. Use this for buttons (otherwise, use
    /// GameManager's static function).
    /// </summary>
    /// <param name="scene"></param>
    public void LoadScene(string scene) {
        GameManager.LoadScene(scene);
    }

    public void QuitGame() {
        GameManager.QuitGame();
    }

    public static void SetHoverText(string s) {
        if (Instance.hoverText != null) { Instance.hoverText.text = s; }
    }

    public static void ClearHoverText() {
        SetHoverText("");
    }

    private static void ShowGameButtons(bool show) {
        if (!show || (!Inventory.isShown && !DialogueManager.isShown)) {
            foreach (GameObject button in Instance.gameButtons) {
                button.SetActive(show);
            }
        }
    }
    
    public static void FadeOut(bool isDialogueAction) { FadeOut(FadeTime, isDialogueAction); }
    public static void FadeOut(float duration = -1f, bool isDialogueAction = false) {
        SetAlpha(Instance.blackout, 0.01f);
        Fade(255f, duration, isDialogueAction);
    }

    public static void FadeIn(bool isDialogueAction) { FadeIn(FadeTime, isDialogueAction); }
    public static void FadeIn(float duration = -1f, bool isDialogueAction = false) {
        SetAlpha(Instance.blackout, 1f);
        Fade(0f, duration, isDialogueAction);
    }

    private static void SetAlpha(Image image, float alpha) {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private static void Fade(float targetAlpha, float duration, bool isDialogueAction = false) {
        Instance.dlgActionFade = isDialogueAction;

        float d = (duration == -1f) ? FadeTime : duration;
        Instance.currentFadeTime = duration;
        Instance.fadeStart = System.DateTime.Now;

        Instance.blackout.gameObject.SetActive(true);
        Instance.blackout.CrossFadeAlpha(targetAlpha, d, false);
    }
}