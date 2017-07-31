using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interactable object that exists within the
/// game world (as opposed to the inventory).
/// </summary>
public class WorldItem : GameItem {
    /// <summary>
    /// Whether this item may be interacted with. Interactable
    /// items should, at the least, have a dialogue attached to
    /// them with examine text.
    /// </summary>
    [SerializeField] [HideInInspector]
    private bool interactable;

    [SerializeField]
    private float minInteractionDistance = 1;

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
    private CustomAIPath aiPath;

    private Collider2D coll;
    
    /// <summary>
    /// Initializes fields.
    /// </summary>
	void Start () {
        seeker = gameObject.GetComponent<Seeker>();
        aiPath = gameObject.GetComponent<CustomAIPath>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        coll = gameObject.GetComponent<Collider2D>();
        
        startingScale = transform.localScale;
        startingZPos = GameManager.ZDepthMap.GetZDepthAtWorldPoint(transform.position);
    }

    void Update() {
        if (aiPath != null && aiPath.isMoving) {
            UpdateScale();
            if (Animator != null) { UpdateWalkDirection(); }
        }
    }
    
    /// <summary>
    /// Trigger player movement and object interaction
    /// when the player clicks this object.
    /// </summary>
    void OnMouseUpAsButton() {
        if (UIManager.WorldInputEnabled) {
            InteractionTarget = this;
            GameManager.Player.MoveToPoint(coll.bounds.ClosestPoint(GameManager.Player.transform.position));
        }
    }

    public override void OnMouseEnter() {
        if (UIManager.WorldInputEnabled) {
            base.OnMouseEnter();
        }
    }

    public override void OnMouseExit() {
        if (UIManager.WorldInputEnabled) {
            base.OnMouseExit();
        }
    }

    public override void Interact() {
        if (interactable) {
            Vector3 playerPos = GameManager.Player.transform.position;
            Vector3 target = coll.bounds.ClosestPoint(playerPos);

            if (Vector2.Distance(playerPos, target) < minInteractionDistance) {
                base.Interact();
            }
        }
    }
    
    /// <summary>
    /// Returns the current position of the instance of this object
    /// in the scene.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPosition() {
        if (Instance != null) {
            return Instance.transform.position;
        }
        else { return Vector3.zero; }
    }

    /// <summary>
    /// Initiate movement to a given point.
    /// </summary>
    public virtual void MoveToPoint(Vector2 point) {
        if (aiPath == null) {
            Debug.LogError("Tried to move " + gameObject.name + ", but it does not have pathfinding AI. " +
                "(Must have Seeker and CustomAIPath components attached.)");
        }
        else {
            if (Animator != null) { Animator.SetInteger("state", (int)AnimState.WALK); }
            seeker.StartPath(transform.position, point);
        }
    }

    /// <summary>
    /// Called when the item successfully arrives at its target.
    /// </summary>
    public virtual void OnTargetReached(Transform target) {
        if (Animator != null) { Animator.SetInteger("state", (int)AnimState.IDLE); }
    }

    /// <summary>
    /// Updates the scale of the object based on its z position.
    /// </summary>
    protected void UpdateScale() {
        float currentZ = GameManager.ZDepthMap.GetZDepthAtWorldPoint(Instance.transform.position);

        float camZ = Camera.main.transform.position.z;
        float zDist = Mathf.Abs(camZ - currentZ);
        float startingZDist = Mathf.Abs(camZ - startingZPos);

        float flipModifier = Instance.transform.localScale.x < 0 ? -1 : 1;

        Vector3 scale = startingScale / (startingZDist / zDist);
        scale.x *= flipModifier;

        transform.localScale = scale;
    }

    protected void UpdateWalkDirection() {
        CardinalDirection dir = aiPath.GetDirection();
        if (dir == CardinalDirection.WEST) { Flip(false); }
        else if (dir == CardinalDirection.EAST) { Flip(true); }
        
        Animator.SetInteger("direction", (int)aiPath.GetDirection());
    }

    /// <summary>
    /// Set this object's sprite.
    /// </summary>
    public override void ChangeSprite(Sprite sprite) {
        if (((WorldItem)Instance).spriteRenderer != null) {
            ((WorldItem)Instance).spriteRenderer.sprite = sprite;
        }
    }

    public void RemoveFromWorld() {
        Destroy(Instance.gameObject);
        World.UpdateNavGraph();
    }

    public void Enable() {
        Instance.gameObject.SetActive(true);
    }

    public void Flip(bool faceRight) {
        Vector3 scale = Instance.transform.localScale;
        bool flip = faceRight ? (scale.x > 0) : (scale.x < 0);

        if (flip) {
            scale.x *= -1;
            Instance.transform.localScale = scale;
        }
    }
}