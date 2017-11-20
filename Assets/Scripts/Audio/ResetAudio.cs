using UnityEngine;

public class ResetAudio : MonoBehaviour {
    void Awake() {
        AkSoundEngine.StopAll();
    }
}