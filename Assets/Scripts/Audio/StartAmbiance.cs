using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAmbiance : AudioManager {

//	GameObject audioManagerObj;
//	AudioManager audioManager;

	// Use this for initialization
	void Start () {
//		audioManagerObj = GameObject.Find ("AudioManager");
//		audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
		PlayEvent ("IncineratorAreaStart");
	}
	
	// Update is called once per frame
	void Update () {
	}
}
