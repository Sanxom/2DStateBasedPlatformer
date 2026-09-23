using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashAbility : BaseAbility
{
    [SerializeField] private InputActionReference dashActionRef;
    [SerializeField] private float dashForce;
    [SerializeField] private float maxDashDuration;
    [SerializeField] private bool canDashFromCrouch;

    private const string DASH_ANIM_PARAMETER_NAME = "dash";

    private float dashTimer;
    private int dashParameterID;

    protected override void Init()
    {
        base.Init();

        dashParameterID = Animator.StringToHash(DASH_ANIM_PARAMETER_NAME);
    }
    
    private void OnEnable()
    {
        dashActionRef.action.performed += TryToDash;
    }

    private void OnDisable()
    {
        dashActionRef.action.performed -= TryToDash;
    }

    public override void EnterAbility()
    {
        linkedPhysics.didWallJump = false;
    }

    public override void ProcessAbility()
    {
        dashTimer -= Time.deltaTime;
        if (linkedPhysics.isWallDetected || linkedPhysics.isSmallWallDetected)
            dashTimer = -1f;

        if (dashTimer <= 0f)
        {
            if (linkedPhysics.isGrounded)
                linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            else
                linkedStateMachine.ChangeState(PlayerStates.State.Jump);
        }
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(dashParameterID, linkedStateMachine.currentState == PlayerStates.State.Dash);
    }

    public override void ExitAbility()
    {
        linkedPhysics.EnableGravity();
        linkedPhysics.ResetVelocity();
    }

    private void TryToDash(InputAction.CallbackContext context)
    {
        if (!isPermitted) return;
        if (linkedStateMachine.currentState == PlayerStates.State.Dash 
            || linkedPhysics.isWallDetected
            || !CanDashFromCrouch()) return;

        linkedStateMachine.ChangeState(PlayerStates.State.Dash);
        linkedPhysics.DisableGravity();
        linkedPhysics.ResetVelocity();

        if (player.isFacingRight)
            linkedPhysics.rb.linearVelocityX = dashForce;
        else
            linkedPhysics.rb.linearVelocityX = -dashForce;

        dashTimer = maxDashDuration;
    }

    private bool CanDashFromCrouch()
    {
        if (!canDashFromCrouch && linkedStateMachine.currentState == PlayerStates.State.Crouch)
            return false;
        else if (canDashFromCrouch && linkedStateMachine.currentState == PlayerStates.State.Crouch)
            return true;
        return true;
    }
}