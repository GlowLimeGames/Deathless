using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class WorldObject : MonoBehaviour {
    public const float DEFAULT_SPEED = 0.25f;
    private Vector3 startingScale;
    private float startingZPos;

    [SerializeField]
    private Dialogue.SerializableTree dialogue;
    private SpriteRenderer spriteRenderer;

    private bool isMoving;
    private List<Vector3> waypoints;
    private float speed;

    //Temporary, for testing
    [SerializeField]
    private InventoryItem inventoryItem;
    
	void Start () {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        startingScale = transform.localScale;
        startingZPos = transform.position.z;
	}

    void Update() {
        if (isMoving) { UpdatePosition(); }
    }
    
    void OnMouseUpAsButton() {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
            GameManager.Player.MoveToPoint(transform.position);
        }

        //Temporary, for testing
        Inventory.AddItem(inventoryItem);
        Destroy(gameObject);
    }

    public void MoveToPoint(Vector2 point, float speed = DEFAULT_SPEED) {
        this.speed = speed;
        waypoints = Pathfinding.GetPath(transform.position, point);
        isMoving = true;
    }

    public void StopMovement() {
        isMoving = false;
        waypoints = null;
    }

    private void UpdatePosition() {
        if (waypoints.Count == 0) {
            isMoving = false;
        }
        else {
            if (transform.position == waypoints[0]) {
                waypoints.Remove(waypoints[0]);
                UpdatePosition();
            }
            else {
                transform.position = Vector3.MoveTowards(transform.position, waypoints[0], speed);
                float camZ = Camera.main.transform.position.z;
                float zDist = Mathf.Abs(camZ - transform.position.z);
                float startingZDist = Mathf.Abs(camZ - startingZPos);
                
                transform.localScale = startingScale * (startingZDist / zDist);
            }
        }
    }

    public void ChangeSprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }
}