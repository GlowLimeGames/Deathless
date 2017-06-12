﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals {
    private static Dictionary<GlobalInt, int> globalInts;
    private static Dictionary<GlobalString, string> globalStrings;

    public static void Init() {
        globalInts = InitDictionary(globalInts);
        globalStrings = InitDictionary(globalStrings);
    }

    private static Dictionary<E, T> InitDictionary<E, T>(Dictionary<E, T> dict) {
        dict = new Dictionary<E, T>();

        foreach (E global in Enum.GetValues(typeof(E))) {
            dict.Add(global, default(T));
        }

        return dict;
    }

    public static string GetGlobal(GlobalString global) {
        return globalStrings[global];
    }

    public static int GetGlobal(GlobalInt global) {
        return globalInts[global];
    }

    public static void SetGlobal(GlobalString global, string value) {
        globalStrings[global] = value;
    }

    public static void SetGlobal(GlobalInt global, int value) {
        globalInts[global] = value;
    }
}

public enum GlobalInt {
    TEST_GLOBAL_INT
}

public enum GlobalString {
    TEST_GLOBAL_STRING
}