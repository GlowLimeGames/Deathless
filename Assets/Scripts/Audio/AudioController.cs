using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** The master audio manager--loads banks and 
 contains functions that will start, stop, pause, resume sound events*/
    
public class AudioController : MonoBehaviour {

    uint bankID;
    //used in stop,resume, pause event functions
    //give value of fadeout to know how much to fade in and out of the sound
    public int fadeout; 
    //load banks; currently will hardcode in bankss
    public void LoadBanks()
    {
        AkSoundEngine.LoadBank("Incinerator", AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID);
    }

    //plays the event 
    public void PlayEvent( string eventName )
    {
        Debug.Log("Currently playing event");
        AkSoundEngine.PostEvent(eventName, gameObject);
        AkSoundEngine.RenderAudio();
        
    }

    //Stops the sound event
    public void StopEvent( string eventName )
    {
        Debug.Log("Stopping event");
        uint eventID; //this is used to stop event in Ak
        eventID = AkSoundEngine.GetIDFromString(eventName);
        //default curve of stopping event is log, but can change later 
        AkSoundEngine.ExecuteActionOnEvent(eventID, AkActionOnEventType.AkActionOnEventType_Stop, gameObject, fadeout * 100, AkCurveInterpolation.AkCurveInterpolation_Log1);
        AkSoundEngine.RenderAudio();
    }

    public void PauseEvent(string eventName)
    {
        Debug.Log("pausing event");
        uint eventID; //this is used to stop event in Ak
        eventID = AkSoundEngine.GetIDFromString(eventName);
        //default curve of stopping event is log, but can change later 
        AkSoundEngine.ExecuteActionOnEvent(eventID, AkActionOnEventType.AkActionOnEventType_Pause, gameObject, fadeout * 100, AkCurveInterpolation.AkCurveInterpolation_Log1);
        AkSoundEngine.RenderAudio();
       
    }

    public void ResumeEvent(string eventName)
    {
        Debug.Log("resuming event");
        uint eventID; //this is used to stop event in Ak
        eventID = AkSoundEngine.GetIDFromString(eventName);
        //default curve of stopping event is log, but can change later 
        AkSoundEngine.ExecuteActionOnEvent(eventID, AkActionOnEventType.AkActionOnEventType_Resume, gameObject, fadeout * 100, AkCurveInterpolation.AkCurveInterpolation_Log1);
        AkSoundEngine.RenderAudio();
    }
}//end of class
