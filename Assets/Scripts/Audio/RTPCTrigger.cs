using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Adjust RTPC of audio when player enters trigger areas
/// </summary>
public class RTPCTrigger : MonoBehaviour {
    [SerializeField]
    private string rtpcName;
    [SerializeField]
    private float rtpcEnterValue;
    [SerializeField]
    private float rtpcExitValue;

    private void OnTriggerEnter2D(Collider2D collision) {
		Debug.Log ("Enter");
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null) {
            AudioController.SetRTPC(rtpcName, rtpcEnterValue);
        }
    }//end of TriggerEnter

    private void OnTriggerExit2D(Collider2D collision) {
		Debug.Log ("Exit");
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null) {
            AudioController.SetRTPC(rtpcName, rtpcExitValue);
        }
    }//end of TriggerExit
}
