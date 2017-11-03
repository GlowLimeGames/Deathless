using UnityEngine;
using System.IO;

public static class BuildProcessing {
    private const string WWISE_PROJECT_PATH = "../Deathless_WwiseProject/";
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
        //get Wwise project file (.wproj) path
        string wwiseProjFile = Path.Combine(Application.dataPath, WwiseSetupWizard.Settings.WwiseProjectPath).Replace('/', Path.DirectorySeparatorChar);
        Debug.Log("Full Wwise project path: " + wwiseProjFile);

        //get Wwise project root folder path
        string wwiseProjectFolder;
        int lastIndex = wwiseProjFile.LastIndexOf(Path.DirectorySeparatorChar);
        if (lastIndex < 0) {
            Debug.LogWarning("Did not find directory separator. Using hardcoded directory instead.");
            wwiseProjectFolder = WWISE_PROJECT_PATH;
        }
        else {
            wwiseProjectFolder = wwiseProjFile.Remove(lastIndex);
        }
        Debug.Log("Wwise folder: " + wwiseProjectFolder);

        string sourceSoundBankFolder = Path.Combine(wwiseProjectFolder, AkBasePathGetter.GetPlatformBasePath());
        string destinationSoundBankFolder = Path.Combine(Application.dataPath + Path.DirectorySeparatorChar + "StreamingAssets",
                                                                Path.Combine(WwiseSetupWizard.Settings.SoundbankPath, wwisePlatformString));

        Debug.Log("Copying soundbanks from " + sourceSoundBankFolder + " to " + destinationSoundBankFolder);
        if (!AkUtilities.DirectoryCopy(sourceSoundBankFolder, destinationSoundBankFolder, true)) {
            Debug.LogError("WwiseUnity: The soundbank folder for the " + wwisePlatformString + " platform doesn't exist. Make sure it was generated in your Wwise project");
        }
        UnityEditor.AssetDatabase.Refresh();
    }
}