using System.Collections;
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

    [SerializeField]
    private GameObject pauseMenu;

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
    private Fadable blackout;

    [SerializeField]
    private Sprite interactIcon;

    [SerializeField]
    private AnimController genericAnimPrefab;
    public static AnimController GenericAnimPrefab { get { return Instance.genericAnimPrefab; } }

    private static Sprite cursorIcon;

    public static bool GamePaused {
        get {
            return (Time.timeScale == 0);
        }
    }

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

    void Awake() {
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

        if ((Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Space))
            && GameManager.GetCurrentScene() != GameManager.MAIN_MENU_SCENE) {
            Pause(!GamePaused);
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

    public void Pause(bool pause) {
        Time.timeScale = pause ? 0 : 1;
        OnShowUIElement(pause);
        pauseMenu.SetActive(pause);
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
    
    public static void FadeOut(bool isDialogueAction) { FadeOut(Fadable.DEFAULT_FADE_RATE, isDialogueAction); }
    public static void FadeOut(float duration = Fadable.DEFAULT_FADE_RATE, bool isDialogueAction = false, Fadable.FadeCallback callback = null) {
        Instance.blackout.gameObject.SetActive(true);
        if (isDialogueAction) { callback += Dialogue.Actions.CompletePendingAction; }
        Instance.blackout.StartFadeIn(duration, callback);
    }
    
    public static void FadeIn(bool isDialogueAction) { FadeIn(Fadable.DEFAULT_FADE_RATE, isDialogueAction); }
    public static void FadeIn(float duration = Fadable.DEFAULT_FADE_RATE, bool isDialogueAction = false, Fadable.FadeCallback callback = null) {
        Instance.blackout.gameObject.SetActive(true);
        callback += CompleteFadeIn;
        if (isDialogueAction) { callback += Dialogue.Actions.CompletePendingAction; }
        Instance.blackout.StartFadeOut(duration, callback);
    }

    private static void CompleteFadeIn() {
        Instance.blackout.gameObject.SetActive(false);
    }
}