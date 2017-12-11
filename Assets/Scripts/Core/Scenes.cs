using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameScene {
    NONE = 0,
    MAIN_MENU,
    INCINERATOR,
    CREDITS
}

public class Scenes : Manager<Scenes> {
    private static Dictionary<GameScene, string> sceneNames = new Dictionary<GameScene, string>() {
        { GameScene.MAIN_MENU, "MainMenu" },
        { GameScene.INCINERATOR, "S1_Incinerator" },
        { GameScene.CREDITS, "Credits" }
    };

    public static GameScene CurrentScene {
        get { return GetScene(SceneManager.GetActiveScene().name); }
    }

    private const float TRANSITION_TIME = 2f;
    private static GameScene lastScene;

    void Awake() {
        if (SingletonInit()) {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    public static GameScene GetScene(string sceneName) {
        return sceneNames.FirstOrDefault(x => x.Value == sceneName).Key;
    }

    public static bool IsCurrentScene(string sceneName) {
        return IsCurrentScene(GetScene(sceneName));
    }

    public static bool IsCurrentScene(GameScene scene) {
        return scene == CurrentScene;
    }

    /// <summary>
    /// This returns true if the scene is considered a playable game
    /// scene (i.e. is not a menu/UI scene).
    /// Note that ALL scenes are listed in the GameScene enum.
    /// </summary>
    public static bool IsGameScene(GameScene scene) {
        return scene != GameScene.MAIN_MENU && scene != GameScene.CREDITS;
    }

    public static void BeginSceneTransition(string sceneName) {
        BeginSceneTransition(GetScene(sceneName));
    }

    public static void BeginSceneTransition(GameScene scene) {
        Instance.StartCoroutine(SceneTransition(scene));
    }

    private static IEnumerator SceneTransition(GameScene scene) {
        SceneAudio sceneAudio = FindObjectOfType<SceneAudio>();
        if (sceneAudio != null) { sceneAudio.DoStopEvents(scene); }

        UIManager.FadeOut(TRANSITION_TIME);
        
        lastScene = CurrentScene;

        yield return new WaitForSecondsRealtime(TRANSITION_TIME);
        
        if (sceneAudio != null) { sceneAudio.EndSceneAudio(scene); }
        if (scene == GameScene.MAIN_MENU) { GameManager.ResetGameData(); }

        LoadSceneImmediately(scene);
    }

    private static void LoadSceneImmediately(GameScene scene) {
        SceneManager.LoadScene(sceneNames[scene]);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneAudio sceneAudio = FindObjectOfType<SceneAudio>();
        if (sceneAudio != null) { sceneAudio.LoadAudio(lastScene); }
        
        GameManager.PlayIntro();
        UIManager.FadeIn(TRANSITION_TIME);
    }
}