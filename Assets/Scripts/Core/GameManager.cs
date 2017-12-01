using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// High-level management of game systems and objects.
/// </summary>
public class GameManager : Manager<GameManager> {
    [SerializeField]
    private bool displayIntroInEditor = false;

    /// <summary>
    /// The Player object in the current scene.
    /// </summary>
    public static Player Player {
        get {
            if (Instance.player == null) {
                Instance.player = FindObjectOfType<Player>();
            }
            return Instance.player;
        }
    }

    /// <summary>
    /// Backing field for Player.
    /// </summary>
    private Player player;

    /// <summary>
    /// The ZDepthMap in the current scene.
    /// </summary>
    public static ZDepthMap ZDepthMap {
        get {
            if (Instance.zDepthMap == null) {
                Instance.zDepthMap = FindObjectOfType<ZDepthMap>();
            }
            return Instance.zDepthMap; }
    }

    /// <summary>
    /// Backing field for ZDepthMap.
    /// </summary>
    private ZDepthMap zDepthMap;
    
	void Awake() {
        SingletonInit();
        Globals.Init();
	}

    public static void PlayIntro() {
        Intro intro = Util.FindObjectOfType<Intro>(true);
        if (intro != null && (Instance.displayIntroInEditor || !Application.isEditor)) {
            intro.gameObject.SetActive(true);
        }
    }

    public static void QuitGame() {
        Application.Quit();
    }

    public static void ResetGameData() {
        DialogueManager.ResetOneShotDialogue();
    }
}