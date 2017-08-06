using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class AnimController : MonoBehaviour {
    private const string
        DIRECTION = "direction",
        ONE_SHOT = "one shot",
        IDLE = "idle",
        WALK = "walk";

    [SerializeField]
    private bool canWalk;

    private string current = IDLE;
    private Animator animator;
    private AnimatorOverrideController animOverride;
    private bool temp = false;
    private bool dlgAction = false;

    private SpriteRenderer spriteRenderer;
    private Sprite idleSprite;
    private CustomAIPath aiPath;
    
    private Vector3 startingScale;
    private float startingZPos;

    void Start() {
        if (animator == null) { Init(); }
    }

    public void InitTemp() {
        temp = true;
        Init();
    }

    void Init() {
        animator = GetComponent<Animator>();
        animOverride = animator.runtimeAnimatorController as AnimatorOverrideController;

        if (animOverride == null) {
            animOverride = new AnimatorOverrideController(animator.runtimeAnimatorController);
        }
        else { animOverride = new AnimatorOverrideController(animOverride); }

        spriteRenderer = GetComponent<SpriteRenderer>();
        aiPath = GetComponent<CustomAIPath>();

        idleSprite = spriteRenderer.sprite;

        startingScale = transform.localScale;
        startingZPos = GameManager.ZDepthMap.GetZDepthAtWorldPoint(transform.position);
    }

    void Update() {
        if (current == WALK && aiPath != null) {
            UpdateScale();
            UpdateWalkDirection(aiPath.GetDirection());
        }
        else if (current == ONE_SHOT && !CurrentStateEquals(ONE_SHOT)) {
            if (dlgAction) { Dialogue.Actions.CompletePendingAction(); }
            if (temp) { Destroy(gameObject); }
            else { Idle(); }
        }
    }

    private bool CurrentStateEquals(string tag) {
       return animator.GetCurrentAnimatorStateInfo(0).IsTag(tag);
    }

    public static void PlayAnimAt(AnimationClip anim, Vector3 pos, bool isDialogueAction = false) {
        AnimController animController = Instantiate(UIManager.GenericAnimPrefab, pos, Quaternion.identity);
        animController.InitTemp();
        animController.PlayOneShot(anim, isDialogueAction);
    }

    public void PlayOneShot(AnimationClip anim, bool isDialogueAction = false) {
        dlgAction = isDialogueAction;
        ReplaceClip(ONE_SHOT, anim);
        animator.SetTrigger(ONE_SHOT);
        current = ONE_SHOT;
    }

    private void ReplaceClip(string replaceName, AnimationClip clip) {
        animOverride[replaceName] = clip;
        animator.runtimeAnimatorController = animOverride;
    }

    public void Idle() {
        if (idleSprite != null) { spriteRenderer.sprite = idleSprite; }
        current = IDLE;
        animator.SetTrigger(IDLE);
    }

    public void Walk() {
        if (canWalk) {
            current = WALK;
            animator.SetTrigger(WALK);
        }
    }

    public void SetIdle(Sprite idle) {
        SetIdle(null, idle);
    }

    public void SetIdle(AnimationClip idle) {
        SetIdle(idle, null);
    }

    private void SetIdle(AnimationClip anim, Sprite sprite) {
        idleSprite = sprite;
        animOverride[IDLE] = anim;
        animator.runtimeAnimatorController = animOverride;
        Idle();
    }

    private void UpdateWalkDirection(CardinalDirection dir) {
        if (dir == CardinalDirection.WEST) { Flip(false); }
        else if (dir == CardinalDirection.EAST) { Flip(true); }

        animator.SetInteger(DIRECTION, (int)dir);
    }

    private void Flip(bool faceRight) {
        Vector3 scale = transform.localScale;
        bool flip = faceRight ? (scale.x > 0) : (scale.x < 0);

        if (flip) {
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// Updates the scale of the object based on its z position.
    /// </summary>
    protected void UpdateScale() {
        float currentZ = GameManager.ZDepthMap.GetZDepthAtWorldPoint(transform.position);

        float camZ = Camera.main.transform.position.z;
        float zDist = Mathf.Abs(camZ - currentZ);
        float startingZDist = Mathf.Abs(camZ - startingZPos);

        float flipModifier = transform.localScale.x < 0 ? -1 : 1;

        Vector3 scale = startingScale / (startingZDist / zDist);
        scale.x *= flipModifier;

        transform.localScale = scale;
    }
}