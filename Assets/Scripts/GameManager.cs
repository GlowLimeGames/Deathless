using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// High-level management of game systems and objects.
/// </summary>
public class GameManager : Manager<GameManager> {
    /// <summary>
    /// The Player object in the current scene.
    /// </summary>
    public static Player Player {
        get { return instance.player; }
    }

    /// <summary>
    /// Backing field for Player.
    /// </summary>
    private Player player;
    
	void Start() {
        SingletonInit();
        InitFields();
        SceneManager.sceneLoaded += OnSceneLoaded;
	}

    /// <summary>
    /// Initialize fields with objects found in the
    /// current scene.
    /// </summary>
    void InitFields() {
        if (player == null) {
            player = FindObjectOfType<Player>();
        }
    }

    /// <summary>
    /// Called when the Unity SceneManager finishes loading a scene.
    /// </summary>>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        InitFields();
    }

    /// <summary>
    /// Load a different game scene.
    /// </summary>
    /// <param name="scene"></param>
    public static void LoadScene(string scene) {
        SceneManager.LoadScene(scene);
    }

    public static string GetCurrentScene() {
        return SceneManager.GetActiveScene().name;
    }
}