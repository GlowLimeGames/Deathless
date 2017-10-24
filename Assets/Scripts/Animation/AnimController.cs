using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class AnimController : MonoBehaviour {
    private const string
        DIRECTION = "direction",    // 0-N, 1-S, 2-E, 3-W
        ONE_SHOT = "one shot",
        IDLE = "idle",
        WALK = "walk";

    [SerializeField]
    private bool canWalk;

    [SerializeField]
    private bool allowFlip = true;

    /// <summary>
    /// Allow animation for walking in the north/south directions.
    /// </summary>
    [SerializeField]
    private bool allowNS;

    private string current = IDLE;
    private Animator animator;
    private AnimatorOverrideController animOverride;
    private bool temp = false;
    private bool dlgAction = false;

    private CustomAIPath aiPath;
    private WorldItem worldItem;

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
        
        aiPath = GetComponent<CustomAIPath>();
        worldItem = GetComponent<WorldItem>();
    }

    void Update() {
        if (current == WALK && aiPath != null) {
            worldItem.UpdateZPos();
            UpdateWalkDirection(aiPath.GetDirection(allowNS));
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
        current = IDLE;
        animator.SetTrigger(IDLE);
    }

    public void Walk() {
        if (canWalk) {
            current = WALK;
            animator.SetTrigger(WALK);
        }
    }

    public void SetIdle(AnimationClip idle) {
        animOverride[IDLE] = idle;
        animator.runtimeAnimatorController = animOverride;
        if (current == IDLE) { Idle(); }
    }

    private void UpdateWalkDirection(CardinalDirection dir) {
        if (dir == CardinalDirection.WEST) { Flip(false); }
        else if (dir == CardinalDirection.EAST) { Flip(true); }

        animator.SetInteger(DIRECTION, (int)dir);
    }

    private void Flip(bool faceRight) {
        if (allowFlip) {
            Vector3 scale = transform.localScale;
            bool flip = faceRight ? (scale.x > 0) : (scale.x < 0);

            if (flip) {
                scale.x *= -1;
                transform.localScale = scale;
            }
        }
    }
}