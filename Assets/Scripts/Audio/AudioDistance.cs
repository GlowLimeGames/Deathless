using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script updates a Wwise RTPC (a parameter) based on how far the obect is
/// from the player character.
/// </summary>
public class AudioDistance : MonoBehaviour {
    /// <summary>
    /// The name of the RTPC. Must match the name as defined in the Wwise project.
    /// </summary>
    [SerializeField]
	private string rtpcName;
	
	void Update () {
		float distanceToPlayer = Vector2.Distance(transform.position, GameManager.Player.transform.position);
		AkSoundEngine.SetRTPCValue(rtpcName, distanceToPlayer);
	}
}