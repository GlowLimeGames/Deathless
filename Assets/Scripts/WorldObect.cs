using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class WorldObect : MonoBehaviour {
    [SerializeField]
    private Dialogue.SerializableTree dialogue;
    private SpriteRenderer spriteRenderer;
    

	// Use this for initialization
	void Start () {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Triggers when the player clicks this object
    void OnMouseUpAsButton() {
        // This should move the player to the object
        // and initiate its dialogue.
    }

    public void MoveToPoint(Vector2 point) {
        // This should have the object move to
        // the given point over time.
    }

    public void ChangeSprite(Sprite sprite) {

    }
}
