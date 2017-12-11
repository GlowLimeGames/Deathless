using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : Manager<UIManager> {
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
    public static Sprite InteractIcon { get { return Instance.interactIcon; } }

    [SerializeField]
    private AnimController genericAnimPrefab;
    public static AnimController GenericAnimPrefab { get { return Instance.genericAnimPrefab; } }

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
        CursorUtil.ConfineCustomCursor();

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
            && Scenes.IsGameScene(Scenes.CurrentScene)) {
            Pause(!GamePaused);
        }
    }
    
    public static void OnShowUIElement(bool show) {
        if (!show && (DialogueManager.isShown || Inventory.isShown)) { return; }

        BlockWorldInput(show);
        ShowGameButtons(!show);
        UpdateCursorHover();

        if (show) {
            Inventory.PopupItem.StopAnimation();
        }
    }

    public static void UpdateCursorHover() {
        CursorUtil.SetInteractionCursor(false);
        ClearHoverText();
        Hoverable hitItem = null;
        
        List<RaycastResult> rayResults = new List<RaycastResult>();
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        EventSystem.current.RaycastAll(pointerData, rayResults);
        if (rayResults.Count > 0) {
            hitItem = rayResults[0].gameObject.GetComponent<Hoverable>();
        }
        else {
            Collider2D hitColl = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (hitColl != null) {
                hitItem = hitColl.GetComponent<Hoverable>();
            }
        }

        if (hitItem != null) {
            hitItem.OnHoverEnter();
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
        if (pauseMenu != null) { pauseMenu.SetActive(pause); }
    }

    /// <summary>
    /// Load a different game scene. Use this for buttons (otherwise, use
    /// Scenes' static function).
    /// </summary>
    public void LoadScene(string sceneName) {
        Scenes.BeginSceneTransition(sceneName);
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
    
    public static void FadeOut(float duration = Fadable.DEFAULT_FADE_RATE, Fadable.FadeCallback callback = null) {
        Instance.blackout.gameObject.SetActive(true);
        Instance.blackout.StartFadeIn(duration, callback);
    }
    
    public static void FadeIn(float duration = Fadable.DEFAULT_FADE_RATE, Fadable.FadeCallback callback = null) {
        Instance.blackout.gameObject.SetActive(true);
        callback += CompleteFadeIn;
        Instance.blackout.StartFadeOut(duration, callback);
    }

    private static void CompleteFadeIn() {
        Instance.blackout.gameObject.SetActive(false);
    }
}