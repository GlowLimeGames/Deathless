using System.Collections.Generic;

public class MenuSceneAudio : SceneAudio {
    protected override List<string> SoundBanks { get { return soundBanks; } }
    protected override List<string> StartEvents { get { return startEvents; } }
    protected override List<string> StopEvents { get { return stopEvents; } }

    private List<string> soundBanks = new List<string>() {
        "Incinerator_MX"
    };

    private List<string> startEvents = new List<string>() {
        "MainMenu_MX_MainTitle"
    };

    private List<string> stopEvents = new List<string>() {
        "MainMenu_MX_StopMainTitle"
    };

    // Override default functionailty so that menu music
    // continues to play through credits scene.
    #region Override defaults
    protected override void DoStartEvents(GameScene prevScene) {
        if (prevScene == GameScene.NONE || Scenes.IsGameScene(prevScene)) {
            base.DoStartEvents(prevScene);
        }
    }

    public override void DoStopEvents(GameScene nextScene) {
        if (Scenes.IsGameScene(nextScene)) {
            base.DoStopEvents(nextScene);
        }
    }

    public override void EndSceneAudio(GameScene nextScene) {
        if (Scenes.IsGameScene(nextScene)) {
            base.EndSceneAudio(nextScene);
        }
    }
    #endregion
}