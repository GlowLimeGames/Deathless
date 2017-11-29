using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameScene {
    MAIN_MENU,
    INCINERATOR,
    CREDITS
}

public class Scenes : MonoBehaviour {
    private Dictionary<GameScene, string> sceneNames = new Dictionary<GameScene, string>() {
        { GameScene.MAIN_MENU, "MainMenu" },
        { GameScene.INCINERATOR, "S1_Incinerator" },
        { GameScene.CREDITS, "Credits" }
    };

    private float transitionLength = 2f;

    void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void DoSceneTransition(GameScene scene) {

    }

    public void LoadSceneImmediately(GameScene scene) {
        SceneManager.LoadScene(sceneNames[scene]);
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

    }
}