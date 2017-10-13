using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {


	uint bankID;
	// Use this for initialization
	void Awake () {
		AkSoundEngine.LoadBank ("Incinerator", AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID);

	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public void PlayEvent (string eventName){
		AkSoundEngine.PostEvent (eventName, gameObject);
	}

	public void StopEvent (string eventName, float fadeout){
//		uint eventID;
//		eventID = AkSoundEngine.GetIDFromString (eventName);
//		AkSoundEngine.ExecuteActionOnEvent (eventID, AkActionOnEventType.AkActionOnEventType_Stop, gameObject, fadeout * 1000, AkCurveInterpolation.AkCurveInterpolation_Sine);

	}

	public void PauseEvent (){

	}
	public void ResumeEvent (){

	}
}
