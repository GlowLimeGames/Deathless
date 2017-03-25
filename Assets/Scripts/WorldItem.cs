using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interactable object that exists within the
/// game world (as opposed to the inventory).
/// </summary>
public class WorldItem : GameItem {
    /// <summary>
    /// The default speed at which objects should move.
    /// </summary>
    public const float DEFAULT_SPEED = 0.25f;

    /// <summary>
    /// The scale of the gameObject on start.
    /// </summary>
    private Vector3 startingScale;

    /// <summary>
    /// The z position of the gameObject on start.
    /// </summary>
    private float startingZPos;
    
    /// <summary>
    /// The sprite renderer attached to the gameObject.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// Whether this object is currently moving.
    /// </summary>
    private bool isMoving;

    /// <summary>
    /// The path to the object's destination. It should
    /// move to each of these waypoints in order.
    /// </summary>
    private List<Vector3> waypoints;

    /// <summary>
    /// The speed at which this object will move.
    /// </summary>
    private float speed;

    /// <summary>
    /// (TEMPORARY, for testing) The inventory item the
    /// player gets when interacting with this object.
    /// </summary>
    [SerializeField]
    private InventoryItem inventoryItem;
    
    /// <summary>
    /// Initializes fields.
    /// </summary>
	void Start () {
        spriteRenderer = Instance.gameObject.GetComponent<SpriteRenderer>();
        startingScale = transform.localScale;
        startingZPos = transform.position.z;
	}

    /// <summary>
    /// Performs movement updates.
    /// </summary>
    void Update() {
        if (isMoving) { UpdatePosition(); }
    }
    
    /// <summary>
    /// Trigger player movement and object interaction
    /// when the player clicks this object.
    /// </summary>
    void OnMouseUpAsButton() {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
            GameManager.Player.MoveToPoint(transform.position);

            Interact();

            //Temporary, for testing
            if (!Inventory.isItemSelected && inventoryItem != null) {
                Inventory.AddItem(inventoryItem);
                Destroy(Instance.gameObject);
            }
        }
    }

    /// <summary>
    /// Initiate movement to a given waypoint object.
    /// </summary>
    public void MoveToPoint(GameObject waypoint) {
        MoveToPoint(waypoint.transform.position);
    }

    /// <summary>
    /// Initiate movement to a given point.
    /// </summary>
    public virtual void MoveToPoint(Vector2 point, float speed = DEFAULT_SPEED) {
        this.speed = speed;
        waypoints = Pathfinding.GetPath(transform.position, point);
        isMoving = true;
    }

    /// <summary>
    /// Halt any in-progress movement for this object.
    /// </summary>
    public void StopMovement() {
        isMoving = false;
        waypoints = null;
    }

    /// <summary>
    /// Updates the current position for a moving object.
    /// </summary>
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

    /// <summary>
    /// Set this object's sprite.
    /// </summary>
    public void ChangeSprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }
}