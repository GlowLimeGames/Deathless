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
    private bool freezeZPosition;

    [SerializeField]
    private float minInteractionDistance = 1;

    [SerializeField]
    private GameObject speechBubble;

    [SerializeField]
    private string footstepAudioEvent = "IncineratorArea_PlayFootsteps";

    private bool dlgActionMovement = false;
    
    /// <summary>
    /// The sprite renderer attached to the gameObject.
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// The Seeker attached to the gameObject. Part of the A* Pathfinding Project plugin.
    /// </summary>
    private Seeker seeker;
    private CustomAIPath aiPath;

    private Collider2D coll;

    private Vector3 startingScale;
    private float startingZPos;

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
        UpdateZPos();
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

    public override void OnHoverEnter() {
        if (interactable && UIManager.WorldInputEnabled) {
            base.OnHoverEnter();
        }
    }

    public override void OnHoverExit() {
        if (interactable && UIManager.WorldInputEnabled) {
            base.OnHoverExit();
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
    public virtual void MoveToPoint(Vector2 point, bool isDialogueAction = false) {
        if (Instance != this) { ((WorldItem)Instance).MoveToPoint(point, isDialogueAction); }
        else {
            if (seeker == null) {
                Debug.LogError("Tried to move " + gameObject.name + ", but it does not have pathfinding AI. " +
                    "(Must have Seeker and CustomAIPath components attached.)");
            }
            else {
                dlgActionMovement = isDialogueAction;
                if (AnimController != null) { AnimController.Walk(); }
                aiPath.canMove = true;
                seeker.StartPath(transform.position, point);

                InvokeRepeating("PlayFootsteps", 0f, 0.3f);
            }
        }
    }
    
    private void PlayFootsteps() {
        AudioController.PlayEvent(footstepAudioEvent);
    }

    public void StopMovement() {
        if (Instance != this) { ((WorldItem)Instance).StopMovement(); }
        else if (aiPath != null) {
            aiPath.StopImmediately();
            if (AnimController != null) { AnimController.Idle(); }
            CancelInvoke("PlayFootsteps");
        }
    }

    public void TeleportToPoint(Vector2 point) {
        if (Instance != this) { ((WorldItem)Instance).TeleportToPoint(point); }
        else {
            StopMovement();
            transform.position = point;
            UpdateZPos();
        }
    }

    /// <summary>
    /// Called when the item successfully arrives at its target.
    /// </summary>
    public virtual void OnTargetReached(Transform target) {
        if (dlgActionMovement) { global::Dialogue.Actions.CompletePendingAction(); }
        StopMovement();
    }

    /// <summary>
    /// Set this object's sprite. Note that this only works for items without animators.
    /// (Animated items must set the idle animation instead.)
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

    public void UpdateZPos(bool updateScale = true) {
        if (!freezeZPosition) {
            float currentZ = GameManager.ZDepthMap.GetZDepthAtWorldPoint(transform.position);
            transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);
            if (updateScale) { UpdateScale(currentZ); }
        }
    }
    
    /// <summary>
    /// Updates the scale of the object based on its z position.
    /// </summary>
    private void UpdateScale(float zPos) {
        float camZ = Camera.main.transform.position.z;
        float zDist = Mathf.Abs(camZ - zPos);
        float startingZDist = Mathf.Abs(camZ - startingZPos);

        float flipModifier = transform.localScale.x < 0 ? -1 : 1;

        Vector3 scale = startingScale * (startingZDist / zDist);
        scale.x *= flipModifier;

        transform.localScale = scale;
    }

    public void ShowSpeechBubble(bool show) {
        if (Instance == this) {
            if (speechBubble != null) {
                speechBubble.SetActive(show);
            }
            else { Debug.LogWarning(gameObject.name + " is speaking but does not have a speech bubble."); }
        }
        else if (Instance != null) { ((WorldItem)Instance).ShowSpeechBubble(show); }
    }
}