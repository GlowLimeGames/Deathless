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
    public const float DEFAULT_SPEED = 5f;

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
    /// The Seeker attached to the gameObject. Part of the A* Pathfinding Project plugin.
    /// </summary>
    private Seeker seeker;

    /// <summary>
    /// The AIPath attached to the gameObject. Part of the A* Pathfinding Project plugin.
    /// </summary>
    private AIPath aiPath;

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
        seeker = gameObject.GetComponent<Seeker>();
        aiPath = gameObject.GetComponent<AIPath>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        startingScale = transform.localScale;

        startingZPos = GameManager.ZDepthMap.GetZDepthAtWorldPoint(transform.position);
    }

    void Update() {
        UpdateScale();
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
    /// Initiate movement to a given point.
    /// </summary>
    public virtual void MoveToPoint(Vector2 point, float speed = DEFAULT_SPEED) {
        aiPath.speed = speed;

        //float z = GameManager.ZDepthMap.GetZDepthAtScreenPoint(Camera.main.WorldToScreenPoint(point));
        //Vector3 target = new Vector3(point.x, point.y, z);

        seeker.StartPath(transform.position, point, OnPathComplete);
    }

    public void OnPathComplete(Pathfinding.Path p) {
        Debug.Log("Yay, we got a path back. The complete state is: " + p.CompleteState);
    }

    /// <summary>
    /// Updates the scale of the object based on its z position.
    /// </summary>
    protected void UpdateScale() {
        if (startingScale != default(Vector3)) {
            float currentZ = GameManager.ZDepthMap.GetZDepthAtWorldPoint(transform.position);

            float camZ = Camera.main.transform.position.z;
            float zDist = Mathf.Abs(camZ - currentZ);
            float startingZDist = Mathf.Abs(camZ - startingZPos);

            transform.localScale = startingScale / (startingZDist / zDist);

            //Debug.Log("Z pos: " + transform.position.z);
            //Debug.Log("Attempted to update scale. zDist: " + zDist + ", startingZDist: " + startingZDist);
        }
        else { Debug.Log("haven't initialized scale"); }
    }

    /// <summary>
    /// Set this object's sprite.
    /// </summary>
    public void ChangeSprite(Sprite sprite) {
        spriteRenderer.sprite = sprite;
    }
}