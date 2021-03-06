﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains static utility functions.
/// </summary>
public static class Util {
    public delegate void WaitCallback();
    private static List<GameObject> dontDestroyOnLoad = new List<GameObject>();

    /// <summary>
    /// Similar to GameObject.FindObjectOfType but allows for
    /// the inclusion of inactive objects.
    /// </summary>
    public static T FindObjectOfType<T>(bool includeInactive) where T : Component {
        T result = null;

        foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
            result = obj.GetComponentInChildren<T>(includeInactive);
            if (result != null) { return result; }
        }

        foreach (GameObject obj in dontDestroyOnLoad) {
            if (obj != null) {
                result = obj.GetComponentInChildren<T>(includeInactive);
                if (result != null) { return result; }
            }
        }

        return result;
    }

    /// <summary>
    /// Similar to GameObject.FindObjectsOfType but allows for
    /// the inclusion of inactive objects.
    /// </summary>
    public static T[] FindObjectsOfType<T>(bool includeInactive) where T : Component {
        List<T> results = new List<T>();

        foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
            results.AddRange(obj.GetComponentsInChildren<T>(includeInactive));
        }
        
        foreach (GameObject obj in dontDestroyOnLoad) {
            if (obj != null) {
                results.AddRange(obj.GetComponentsInChildren<T>(includeInactive));
            }
        }

        return results.ToArray();
    }

    public static void DontDestroyOnLoad(GameObject obj) {
        dontDestroyOnLoad.Add(obj);
        GameObject.DontDestroyOnLoad(obj);
    }

    public static void Destroy(GameObject obj) {
        dontDestroyOnLoad.Remove(obj);
        GameObject.Destroy(obj);
    }

    public static void Wait(MonoBehaviour obj, float seconds, WaitCallback callback = null) {
        obj.StartCoroutine(WaitEnum(seconds, callback));
    }

    private static IEnumerator WaitEnum (float seconds, WaitCallback callback) {
        yield return new WaitForSeconds(seconds);
        callback.Invoke();
    }
}