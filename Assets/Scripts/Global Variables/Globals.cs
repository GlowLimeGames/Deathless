using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals {
    private static Dictionary<GlobalInt, int> globalInts;
    private static Dictionary<GlobalString, string> globalStrings;
    private static Dictionary<GlobalBool, bool> globalBools;

    public static void Init() {
        globalInts = InitDictionary(globalInts);
        globalStrings = InitDictionary(globalStrings);
        globalBools = InitDictionary(globalBools);
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

    public static bool GetGlobal(GlobalBool global) {
        return globalBools[global];
    }

    public static void SetGlobal(GlobalString global, string value) {
        globalStrings[global] = value;
    }

    public static void SetGlobal(GlobalInt global, int value) {
        globalInts[global] = value;
    }

    public static void SetGlobal(GlobalBool global, bool value) {
        globalBools[global] = value;
    }
}

public enum GlobalInt {
    TEST_GLOBAL_INT
}

public enum GlobalString {
    TEST_GLOBAL_STRING
}

public enum GlobalBool {
    TEST_GLOBAL_BOOL,
    S1_GUARDS_DISTRACTED,
    S1_ALCOHOL_TAKEN,
    S1_RUST_BOOZED,
    S1_RUST_BURNT,
    S1_VAT_TOPPLED,
    S1_CAUSED_EXPLOSION,
    S1_KILLED_GUARD
}