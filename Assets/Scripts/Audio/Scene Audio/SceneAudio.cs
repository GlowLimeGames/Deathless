using System.Collections.Generic;
using UnityEngine;

public abstract class SceneAudio : MonoBehaviour {
    protected abstract List<string> SoundBanks { get; }
    protected abstract List<string> StartEvents { get; }
    protected abstract List<string> StopEvents { get; }

    public void LoadAudio(GameScene prevScene) {
        LoadSoundBanks(prevScene);
        DoStartEvents(prevScene);
    }

    protected virtual void LoadSoundBanks(GameScene prevScene) {
        foreach (string soundBank in SoundBanks) {
            AudioController.LoadSoundBank(soundBank);
        }
    }

    protected virtual void DoStartEvents(GameScene prevScene) { DoEvents(StartEvents); }
    public virtual void DoStopEvents(GameScene nextScene) { DoEvents(StopEvents); }

    private void DoEvents(List<string> audioEvents) {
        foreach (string audioEvent in audioEvents) {
            AudioController.PlayEvent(audioEvent);
        }
    }

    public virtual void EndSceneAudio(GameScene nextScene) {
        AkSoundEngine.StopAll();
    }
}