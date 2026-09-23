using UnityEngine;
using UnityEngine.InputSystem;

public class WallJumpAbility : BaseAbility
{
    public InputActionReference wallJumpActionRef;

    [SerializeField] private Vector2 wallJumpForce;
    [SerializeField] private float wallJumpMaxTime;

    private float wallJumpMinTime;
    private float wallJumpTimer;

    private void OnEnable()
    {
        wallJumpActionRef.action.performed += TryToWallJump;
    }

    private void OnDisable()
    {
        wallJumpActionRef.action.performed -= TryToWallJump;
    }

    protected override void Init()
    {
        base.Init();
        wallJumpTimer = wallJumpMaxTime;
    }

    public override void EnterAbility()
    {
        linkedPhysics.didWallJump = false;
    }

    public override void ProcessAbility()
    {
        wallJumpTimer -= Time.deltaTime;
        wallJumpMinTime -= Time.deltaTime;

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
            player.ForceFlip();
            if (player.isFacingRight)
                linkedPhysics.rb.linearVelocity = new(wallJumpForce.x, wallJumpForce.y);
            else
                linkedPhysics.rb.linearVelocity = new(-wallJumpForce.x, wallJumpForce.y);
        }
    }
}