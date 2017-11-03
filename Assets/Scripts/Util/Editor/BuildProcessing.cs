using UnityEngine;
using System.IO;

public static class BuildProcessing {
    private const string MAC = "Mac";
    private const string WINDOWS = "Windows";

    // PreBuild methods are called from Unity Cloud Build
    // Build target -> Advanced Options -> Pre-Export Method
    [UnityEditor.MenuItem("Assets/Wwise/Copy Soundbanks to Project/Mac")]
    public static void PreBuildMac() { MoveSoundBanks(MAC); }

    [UnityEditor.MenuItem("Assets/Wwise/Copy Soundbanks to Project/Windows")]
    public static void PreBuildWindows() { MoveSoundBanks(WINDOWS); }

    /// <summary>
    /// Based on AkExampleAppBuilderBase.Build
    /// </summary>
    private static void MoveSoundBanks(string wwisePlatformString) {
        string wwiseProjFile = Path.Combine(Application.dataPath, WwiseSetupWizard.Settings.WwiseProjectPath).Replace('/', Path.DirectorySeparatorChar);
        string wwiseProjectFolder = wwiseProjFile.Remove(wwiseProjFile.LastIndexOf(Path.DirectorySeparatorChar));

        string sourceSoundBankFolder = Path.Combine(wwiseProjectFolder, AkBasePathGetter.GetPlatformBasePath());
        string destinationSoundBankFolder = Path.Combine(Application.dataPath + Path.DirectorySeparatorChar + "StreamingAssets",
                                                         Path.Combine(WwiseSetupWizard.Settings.SoundbankPath, wwisePlatformString));

        Debug.Log("Copying soundbanks from: " + sourceSoundBankFolder + "\nto: " + destinationSoundBankFolder);
        if (!AkUtilities.DirectoryCopy(sourceSoundBankFolder, destinationSoundBankFolder, true)) {
            Debug.LogError("WwiseUnity: The soundbank folder for the " + wwisePlatformString + " platform doesn't exist. Make sure it was generated in your Wwise project");
        }
        UnityEditor.AssetDatabase.Refresh();
    }
}