using System.Collections.Generic;

public class IncineratorSceneAudio : SceneAudio {
    protected override List<string> SoundBanks { get { return soundBanks; } }
    protected override List<string> StartEvents { get { return startEvents; } }
    protected override List<string> StopEvents { get { return stopEvents; } }

    private List<string> soundBanks = new List<string>() {
        "Incinerator_MX",
        "Incinerator_SFX",
        "Incinerator_SFX_Events"
    };

    private List<string> startEvents = new List<string>() {
        "IncineratorArea_SFX_Start",
        "IncineratorArea_MX_Amb",
        "IncineratorArea_SFX_GhostSob",
        "IncineratorArea_SFX_VatAmb",
        "IncineratorArea_SFX_Incinerators"

        // Note: disable IncineratorArea_SFX_Incinerators if you have insufficient memory error while testing.
        // Works fine when starting from main menu.
        // Hopefully audio compression will fix this issue.
    };

    private List<string> stopEvents = new List<string>() {
    };
}