using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager : MonoBehaviour { }

public abstract class Manager<T> : Manager where T : Manager<T> {
    private static T instance;

    /// <summary>
    /// The instance of the manager in the current scene.
    /// </summary>
    protected static T Instance {
        get {
            if (instance == null) {
                Instance = FindObjectOfType<T>();
            }
            return instance;
        }
        private set {
            instance = value;
            if (instance.dontDestroyOnLoad && Application.isPlaying) {
                Util.DontDestroyOnLoad(instance.gameObject);
            }
        }
    }

    [SerializeField]
    private bool dontDestroyOnLoad;
    [SerializeField]
    private bool destroyOld = false;
    
	void Awake () {
        SingletonInit();
	}

    /// <summary>
    /// Returns true if this instance of the object was successfully
    /// initialized.
    /// </summary>
    protected bool SingletonInit() {
        if (instance == null) {
            Instance = (T)this;
        }
        else if (Instance != this) {
            if (destroyOld) { Destroy(); }
            else {
                GameObject.DestroyImmediate(gameObject);
                return false;
            }
        }
        return true;
    }
	
	public static void Destroy() {
        Util.Destroy(Instance.gameObject);
    }
}