using UnityEngine;
using System.IO;

[System.Serializable]
public class BuildManifestData {
    public string scmCommitId;
    public string scmBranch;
    public string buildNumber;
    public string buildStartTime;
    public string projectId;
    public string bundleId;
    public string unityVersion;
    public string xcodeVersion;
    public string cloudBuildTargetName;
}

public static class BuildProcessing {
    private const string MAC = "Mac";
    private const string WINDOWS = "Windows";

    private static BuildManifestData buildManifest;
    public static BuildManifestData BuildManifest {
        get {
            if (buildManifest == null) {
                buildManifest = LoadBuildManifest();
            }
            return buildManifest;
        }
    }

    // PreBuild methods are called from Unity Cloud Build
    // Build target -> Advanced Options -> Pre-Export Method
    [UnityEditor.MenuItem("File/Pre-Build/Mac")]
    public static void PreBuildMac() { PreBuild(MAC); }

    [UnityEditor.MenuItem("File/Pre-Build/Windows")]
    public static void PreBuildWindows() { PreBuild(WINDOWS); }

    private static void PreBuild(string platform) {
        SetVersionNumber();
        MoveSoundBanks(platform);
    }

    // From https://gist.github.com/mattiaswargren/e50ba1b8a9c2b5449da3143f0d1b2816
    private static BuildManifestData LoadBuildManifest() {
        BuildManifestData manifest = null;
        TextAsset json = (TextAsset)Resources.Load("UnityCloudBuildManifest.json");
        if (json != null) {
            manifest = JsonUtility.FromJson<BuildManifestData>(json.text);
            Debug.Log("Attempted to load build manifest from .json file.\n"
                + "build #: " + manifest.buildNumber
                + "\nstart time: " + manifest.buildStartTime
                + "\ntarget name: " + manifest.cloudBuildTargetName);
        }
        else {
            Debug.Log("Failed to load build manifest .json file as TextAsset.");
        }
        return manifest;
    }

    private static void SetVersionNumber() {
        if (BuildManifest != null) {
            Version.buildNumber = BuildManifest.buildNumber;
        }
        else { Debug.Log("Could not set build # as this is not a Unity Cloud Build."); }
    }
    
    // Based on AkExampleAppBuilderBase.Build
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