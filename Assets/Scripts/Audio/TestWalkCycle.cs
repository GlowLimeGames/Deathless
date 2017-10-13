using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWalkCycle : MonoBehaviour {


	public float nextActionTime = 0.0f;
	public float period = 0.5f;

	
	// Update is called once per frame
	void Update () {
		if (Time.deltaTime > nextActionTime) {
			nextActionTime += period;
			Debug.Log ("walkCycle");
			AkSoundEngine.PostEvent ("IncineratorArea_Footsteps",gameObject);
		}
	}
}
