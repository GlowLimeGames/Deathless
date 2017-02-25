using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager instance;

    public static WorldObject Player {
        get { return instance.player; }
    }
    [SerializeField]
    private WorldObject player;
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
	}

    void Update() {
        if (Input.GetMouseButtonUp(1)) {
            if (Inventory.ItemSelected(null)) {
                inventory.gameObject.SetActive(!inventory.gameObject.activeInHierarchy);
            }
            else {
                Inventory.ClearSelection();
            }
        }
    }
}