using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditEscape : MonoBehaviour {

	void LoadMenu() {
		if (Input.GetKeyDown (KeyCode.Escape) == true) {
			SceneManager.LoadScene ("MainMenu");
		} 
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		LoadMenu ();
	}
}
