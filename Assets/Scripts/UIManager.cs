using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    /// <summary>
    /// The instance of UIManager in the current scene.
    /// </summary>
    private static UIManager instance;

    /// <summary>
    /// The Inventory UI object in the current scene.
    /// </summary>
    [SerializeField]
    private Inventory inventory;
    
    /// <summary>
    /// Whether UIManager-controlled input is allowed.
    /// Should be disabled during dialogue.
    /// </summary>
    public static bool InputEnabled { get; set; }

    void Start() {
        instance = this;
        InputEnabled = true;
        if (inventory != null) { inventory.Init(); }
    }

    void Update() {
        // Handle generic input
        if (InputEnabled && inventory != null && Input.GetMouseButtonUp(1)) {
            if (!Inventory.isItemSelected && !Inventory.ObserveIconSelected) {
                Inventory.Show(!Inventory.isShown);
            }
            else {
                Inventory.ClearSelection();
            }
        }
    }
}