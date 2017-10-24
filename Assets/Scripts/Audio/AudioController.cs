using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The master audio manager - loads banks and 
/// contains functions that will start, stop, pause, resume sound events.
/// </summary>
public class AudioController : Manager<AudioController> {
    private const int DEFAULT_FADEOUT = 100;
    
    void Start() {
        LoadBanks();
    }

    /// <summary>
    /// Load banks; currently will hardcode in banks.
    /// Possibly take in a string - name of the soundbank.
    /// </summary>
    private static void LoadBanks() {
        uint bankID;
        AkSoundEngine.LoadBank("Incinerator", AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID);
    }

    private static void DoActionOnEvent(string eventName, AkActionOnEventType action, int fadeout) {
        uint eventID = AkSoundEngine.GetIDFromString(eventName);
        
        //default curve of event action is log, but can change later 
        AkSoundEngine.ExecuteActionOnEvent(eventID, action, instance.gameObject, fadeout, AkCurveInterpolation.AkCurveInterpolation_Log1);
        AkSoundEngine.RenderAudio();
    }
    
    public static void PlayEvent(string eventName) {
        AkSoundEngine.PostEvent(eventName, instance.gameObject);
        AkSoundEngine.RenderAudio();
    }
    
    public static void StopEvent(string eventName, int fadeout = DEFAULT_FADEOUT) {
        DoActionOnEvent(eventName, AkActionOnEventType.AkActionOnEventType_Stop, fadeout);
    }

    public static void PauseEvent(string eventName, int fadeout = DEFAULT_FADEOUT) {
        DoActionOnEvent(eventName, AkActionOnEventType.AkActionOnEventType_Pause, fadeout);
    }

    public static void ResumeEvent(string eventName, int fadeout = DEFAULT_FADEOUT) {
        DoActionOnEvent(eventName, AkActionOnEventType.AkActionOnEventType_Resume, fadeout);
    }
}