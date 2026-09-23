using UnityEngine;

public class WallSlideAbility : BaseAbility
{
    [SerializeField] private float maxSlideSpeed;

    private const string WALL_SLIDE_ANIM_PARAMETER_NAME = "wallSlide";

    private int wallSlideParameterID;

    protected override void Init()
    {
        base.Init();
        wallSlideParameterID = Animator.StringToHash(WALL_SLIDE_ANIM_PARAMETER_NAME);
    }

    public override void EnterAbility()
    {
        linkedPhysics.rb.linearVelocity = Vector2.zero;
        linkedPhysics.didWallJump = false;
        linkedPhysics.hasDashReset = true;
    }

    public override void ProcessAbility()
    {
        if (linkedPhysics.isGrounded)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            return;
        }

        if (player.isFacingRight && linkedInput.horizontalInput < 0f
            || !player.isFacingRight && linkedInput.horizontalInput > 0f)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.isWallDetectedUpper = false;
            linkedPhysics.isWallDetected = false;
            linkedAnimator.SetBool(wallSlideParameterID, false);
            return;
        }

        if (!linkedPhysics.isWallDetectedUpper)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            return;
        }
    }

    public override void ProcessFixedAbility()
    {
        linkedPhysics.rb.linearVelocityY = Mathf.Clamp(linkedPhysics.rb.linearVelocityY, -maxSlideSpeed, 1f);
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(wallSlideParameterID, linkedStateMachine.currentState == PlayerStates.State.WallSlide);
    }
}