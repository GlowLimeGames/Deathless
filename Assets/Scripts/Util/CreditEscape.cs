using UnityEngine;

public class CreditEscape : MonoBehaviour {
	void LoadMenu() {
		if (Input.GetKeyDown (KeyCode.Escape) == true) {
            SceneTransitionManager.BeginSceneTransition(GameScene.MAIN_MENU);
		} 
	}

	void Update () {
		LoadMenu ();
	}
}