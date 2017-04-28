using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// High-level management of game systems and objects.
/// </summary>
public class GameManager : MonoBehaviour {
    /// <summary>
    /// The instance of GameManager in the current scene.
    /// </summary>
    private static GameManager instance;

    /// <summary>
    /// The Player object in the current scene.
    /// </summary>
    public static Player Player {
        get { return instance.player; }
    }

    /// <summary>
    /// Backing field for Player.
    /// </summary>
    [SerializeField]
    private Player player;

    /// <summary>
    /// The Inventory object in the current scene.
    /// </summary>
    [SerializeField]
    private Inventory inventory;

    /// <summary>
    /// Whether GameManager-controlled input is allowed.
    /// Should be disabled during dialogue.
    /// </summary>
    public static bool InputEnabled { get; set; }
    
	void Start () {
        // Singleton
		if (instance == null) {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else {
            Destroy(this);
        }
        
        inventory.Init();
        InputEnabled = true;
	}

    void Update() {
        // Handle generic input
        if (InputEnabled && Input.GetMouseButtonUp(1)) {
            if (!Inventory.isItemSelected && !Inventory.ObserveIconSelected) {
                Inventory.Show(!Inventory.isShown);
            }
            else {
                Inventory.ClearSelection();
            }
        }
    }

    public void LoadScene(string scene) {
        SceneManager.LoadScene(scene);
    }
}