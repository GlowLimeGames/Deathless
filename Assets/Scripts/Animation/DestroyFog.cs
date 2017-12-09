using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script used to fade away fog once door has opened
/// </summary>
public class DestroyFog : Fadable {
	[SerializeField]
	public float fadeRate;

	void Update() {
		GameObject door_open = GameObject.Find ("Door_open");
		if (door_open != null) {
			StartCoroutine (FadeFog ());
		}
	}//end of Update

	private IEnumerator FadeFog(){
		yield return StartFadeOut(fadeRate, null, false);
	}//end of FadeFog
}
