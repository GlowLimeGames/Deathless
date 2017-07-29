using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Manager<UIManager> {
    private const string mainMenuScene = "MainMenu";

    /// <summary>
    /// The Inventory UI object in the current scene.
    /// </summary>
    [SerializeField]
    private Inventory inventory;

    [SerializeField]
    private Text hoverText;

    /// <summary>
    /// Whether world input (clicking items in the world such
    /// as characters, object, and the background) is enabled.
    /// </summary>
    public static bool WorldInputEnabled { get; set; }

    void Start() {
        SingletonInit();

        WorldInputEnabled = true;
        ClearHoverText();
        if (inventory != null) { inventory.Init(); }
    }

    void Update() {
        // Handle generic input
        if (inventory != null && Input.GetMouseButtonUp(1)) {
            if (Inventory.SelectedItem == null) {
                Inventory.Show(!Inventory.isShown);
            }
            else {
                Inventory.ClearSelection();
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape) && GameManager.GetCurrentScene() != mainMenuScene) {
            GameManager.LoadScene(mainMenuScene);
        }
    }

    public static void BlockInput(bool block) {
        WorldInputEnabled = (!block);
    }

    /// <summary>
    /// Load a different game scene. Use this for buttons (otherwise, use
    /// GameManager's static function).
    /// </summary>
    /// <param name="scene"></param>
    public void LoadScene(string scene) {
        GameManager.LoadScene(scene);
    }

    public static void SetHoverText(string s) {
        instance.hoverText.text = s;
    }

    public static void ClearHoverText() {
        SetHoverText("");
    }

    public static void ShowHoverText(bool show) {
        instance.hoverText.gameObject.SetActive(show);
    }
}