using UnityEngine;
using UnityEngine.InputSystem;

public class WallJumpAbility : JumpAbility
{
    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpMaxTime;

    private float wallJumpMinTime;
    private float wallJumpTimer;

    private void OnEnable()
    {
        jumpActionRef.action.performed += TryToWallJump;
    }

    private void OnDisable()
    {
        jumpActionRef.action.performed -= TryToWallJump;
    }

    protected override void Init()
    {
        base.Init();
        wallJumpTimer = wallJumpMaxTime;
    }

    public override void EnterAbility()
    {
        linkedPhysics.didWallJump = false;
        linkedPhysics.hasDashReset = true;
    }

    public override void ProcessAbility()
    {
        wallJumpTimer -= Time.deltaTime;
        wallJumpMinTime -= Time.deltaTime;

        if (wallJumpMinTime < 0f && linkedPhysics.isGrounded)
        {
            if (linkedInput.horizontalInput != 0f)
                linkedStateMachine.ChangeState(PlayerStates.State.Run);
            else
                linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            return;
        }

        if (wallJumpTimer <= 0f)
        {
            if (linkedPhysics.isGrounded)
                linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            else
                linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            return;
        }

        if (wallJumpMinTime <= 0f && linkedPhysics.isWallDetectedUpper)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.WallSlide);
            wallJumpTimer = -1f;
            return;
        }
    }

    public override void ExitAbility()
    {
        linkedPhysics.didWallJump = true;
    }

    private bool EvalWallJumpConditions()
    {
        return !linkedPhysics.isGrounded && linkedPhysics.isWallDetected;
    }

    private void TryToWallJump(InputAction.CallbackContext context)
    {
        if (!isPermitted) return;

        if (EvalWallJumpConditions())
        {
            linkedStateMachine.ChangeState(PlayerStates.State.WallJump);
            wallJumpTimer = wallJumpMaxTime;
            wallJumpMinTime = 0.15f;
            numJumps = maxNumJumps;
            canActivateAdditionalJumps = true;
            player.ForceFlip();
            if (player.isFacingRight)
                linkedPhysics.rb.linearVelocity = new(wallJumpForce.x, wallJumpForce.y);
            else
                linkedPhysics.rb.linearVelocity = new(-wallJumpForce.x, wallJumpForce.y);
            numJumps--;
        }
    }
}