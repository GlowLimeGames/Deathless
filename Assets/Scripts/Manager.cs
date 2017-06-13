using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager : MonoBehaviour { }

public abstract class Manager<T> : Manager where T : Manager<T> {
    /// <summary>
    /// The instance of the manager in the current scene.
    /// </summary>
    protected static T instance;

    [SerializeField]
    private bool dontDestroyOnLoad;
    
	void Awake () {
        SingletonInit();
	}

    protected void SingletonInit() {
        if (instance == null) {
            instance = (T)this;
            if (dontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (instance != this) {
            Destroy();
        }
    }
	
	public static void Destroy() {
        Destroy(instance.gameObject);
    }
}