using System;
using System.Collections.Generic;

/// <summary>
/// A list of all Global boolean variables.
/// PLEASE NOTE that it is important that the numeric value
/// of each entry is not changed.
/// </summary>
public enum GlobalBool {
    S1_GUARDS_DISTRACTED = 1,
    S1_ALCOHOL_TAKEN = 2,
    S1_RUST_BOOZED = 3,
    S1_RUST_BURNT = 4,
    S1_VAT_TOPPLED = 5,
    S1_CAUSED_EXPLOSION = 6,
    S1_KILLED_GUARD = 7,
    S1_KILLED_GHOST = 8,
    S1_AMBROSE_INTRO_TOPIC_ESCAPE = 9,
    S1_AMBROSE_INTRO_TOPIC_DESTROY = 10,
    S1_CULTIST_INTRO_COMPLETE = 11,
    S1_TOPIC_ARK = 12,
    S1_TOPIC_MORTALIST = 13
}

/// <summary>
/// A list of all Global int variables.
/// PLEASE NOTE that it is important that the numeric value
/// of each entry is not changed.
/// </summary>
public enum GlobalInt {
    S1_JASON_DLG_STATE = 0
}

/// <summary>
/// A list of all Global string variables.
/// PLEASE NOTE that it is important that the numeric value
/// of each entry is not changed.
/// </summary>
public enum GlobalString {
}

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