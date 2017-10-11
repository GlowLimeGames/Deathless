using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class testUnityEvent : MonoBehaviour {
    public UnityEvent testEvent; 
	// Use this for initialization

	
	// Update is called once per frame
	void Update () {
        //testing unityevent
		if ( Input.GetKeyDown( KeyCode.E ))
        {
            testEvent.Invoke();
        }
	}
}
