using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script used to fade away fog once door has opened
/// </summary>
public class DestroyFog : MonoBehaviour {

	void Update() {
		GameObject door_open = GameObject.Find ("Door_open");
		if (door_open != null) {
			gameObject.SetActive (false);
		}

	}
}
