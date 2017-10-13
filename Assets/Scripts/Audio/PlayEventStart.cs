using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEventStart : AudioManager {

	public string eventName;

	// Use this for initialization
	void Start () {
		AkSoundEngine.PostEvent (eventName, gameObject);
	}

}
