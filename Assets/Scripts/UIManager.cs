using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Manager<UIManager> {
    private const string mainMenuScene = "MainMenu";

    /// <summary>
    /// The Inventory UI object in the current scene.
    /// </summary>
    [SerializeField]
    private Inventory inventory;

    /// <summary>
    /// The Input Blocker UI object in the current scene.
    /// </summary>
    [SerializeField]
    private GameObject inputBlocker;

    /// <summary>
    /// Whether world input (clicking items in the world such
    /// as characters, object, and the background) is enabled.
    /// </summary>
    public static bool InputEnabled { get; set; }

    void Start() {
        SingletonInit();

        InputEnabled = true;
        if (inventory != null) { inventory.Init(); }
    }

    void Update() {
        // Handle generic input
        if (InputEnabled && inventory != null && Input.GetMouseButtonUp(1)) {
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
        instance.inputBlocker.SetActive(block);
    }

    /// <summary>
    /// Load a different game scene. Use this for buttons (otherwise, use
    /// GameManager's static function).
    /// </summary>
    /// <param name="scene"></param>
    public void LoadScene(string scene) {
        GameManager.LoadScene(scene);
    }
}