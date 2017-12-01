using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameScene {
    MAIN_MENU,
    INCINERATOR,
    CREDITS
}

public class SceneTransitionManager : Manager<SceneTransitionManager> {
    private static Dictionary<GameScene, string> sceneNames = new Dictionary<GameScene, string>() {
        { GameScene.MAIN_MENU, "MainMenu" },
        { GameScene.INCINERATOR, "S1_Incinerator" },
        { GameScene.CREDITS, "Credits" }
    };

    private static float transitionLength = 1f;
    
    private static GameScene lastScene;

    public static void Init() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public static GameScene GetScene(string sceneName) {
        return sceneNames.FirstOrDefault(x => x.Value == sceneName).Key;
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
        
        lastScene = GetScene(SceneManager.GetActiveScene().name);
        Debug.Log("Running scene transition: " + lastScene + " -> " + scene);

        yield return new WaitForSecondsRealtime(transitionLength);
        
        if (sceneAudio != null) { sceneAudio.EndSceneAudio(scene); }
        LoadSceneImmediately(scene);
    }

    private static void LoadSceneImmediately(GameScene scene) {
        SceneManager.LoadScene(sceneNames[scene]);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneAudio sceneAudio = FindObjectOfType<SceneAudio>();
        if (sceneAudio != null) { sceneAudio.LoadAudio(lastScene); }

        Debug.Log("last scene: " + lastScene);
    }
}