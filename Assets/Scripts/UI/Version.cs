using UnityEngine;
using UnityEngine.UI;

public class Version : MonoBehaviour {
    private const string LABEL = "Build";
    public static string buildNumber = "xx";
    
    void Start () {
        Text text = GetComponent<Text>();
        string platform;

        switch (Application.platform) {
            case RuntimePlatform.WindowsPlayer:
                platform = "Windows";
                break;
            case RuntimePlatform.OSXPlayer:
                platform = "Mac";
                break;
            default:
                platform = "plat";
                break;
        }

        text.text = LABEL + " " + buildNumber + " " + platform;
	}
}