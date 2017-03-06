using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	}

    void Update() {
        // Handle generic input
        if (Input.GetMouseButtonUp(1)) {
            if (!Inventory.isItemSelected && !Inventory.ObserveIconSelected) {
                Inventory.Show(!Inventory.isShown);
            }
            else {
                Inventory.ClearSelection();
            }
        }
    }
}