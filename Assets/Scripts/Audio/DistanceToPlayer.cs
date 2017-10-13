using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceToPlayer : MonoBehaviour {
	
	public string rtpcNumber;					//Determines the Number of the RTPC (Check WWise Project's RTPC's)

	private string rtpcName;					//Will Hold the name of the RTPC
	private float distanceToPlayer;				//Will be used to store the Distance from Player to Object
	private Vector2 playerPos;					//Will be used to store a Vector2 of the Player's Position
	private Vector2 objPos;						//Will be used to store a Vector2 of the object's Position
		
	GameObject player;							//Will Hold a Reference to the Player's Object


	// Use this for initialization
	void Start () {

		///Setting RTPC Name, a reference to the Player's Object and the position of the object
		rtpcName = "DistanceToCorpse_" + rtpcNumber;
		player = GameObject.Find ("Player");
		objPos = this.transform.position;

		
	}
	
	// Update is called once per frame
	void Update () {
		///References the Player's position, as well as the distance from the Player to the Object
		playerPos = player.transform.position;
		distanceToPlayer = Vector2.Distance (objPos, playerPos);

		///Sets the RTPC Value
		AkSoundEngine.SetRTPCValue (rtpcName, distanceToPlayer);

	}
}
