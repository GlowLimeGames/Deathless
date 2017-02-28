using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager instance;

    public static Player Player {
        get { return instance.player; }
    }
    [SerializeField]
    private Player player;
    [SerializeField]
    private Inventory inventory;
    

	void Start () {
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