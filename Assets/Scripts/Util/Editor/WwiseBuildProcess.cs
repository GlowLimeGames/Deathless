using UnityEngine;
using System.IO;

public class WwiseBuildProcess {
    private const string MAC = "Mac";
    private const string WINDOWS = "Windows";

    public static void PreBuildMac() { MoveSoundBanks(MAC); }
    public static void PreBuildWindows() { MoveSoundBanks(WINDOWS); }

    /// <summary>
    /// Based on AkExampleAppBuilderBase.Build
    /// </summary>
    private static void MoveSoundBanks(string wwisePlatformString) {
        string wwiseProjFile = Path.Combine(Application.dataPath, WwiseSetupWizard.Settings.WwiseProjectPath).Replace('/', '\\');
        string wwiseProjectFolder = wwiseProjFile.Remove(wwiseProjFile.LastIndexOf(Path.DirectorySeparatorChar));
        
        string sourceSoundBankFolder = Path.Combine(wwiseProjectFolder, AkBasePathGetter.GetPlatformBasePath());
        string destinationSoundBankFolder = Path.Combine(Application.dataPath + Path.DirectorySeparatorChar + "StreamingAssets",
                                                                Path.Combine(WwiseSetupWizard.Settings.SoundbankPath, wwisePlatformString));

        Debug.Log("Moving soundbanks from " + sourceSoundBankFolder + " to " + destinationSoundBankFolder);
        if (!AkUtilities.DirectoryCopy(sourceSoundBankFolder, destinationSoundBankFolder, true)) {
            Debug.LogError("WwiseUnity: The soundbank folder for the " + wwisePlatformString + " platform doesn't exist. Make sure it was generated in your Wwise project");
        }
    }
}