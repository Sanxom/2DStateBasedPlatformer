using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClimbAbility : BaseAbility
{
    public bool canClimb;

    [SerializeField] private InputActionReference climbActionRef;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float setMinClimbTime;
    [SerializeField] private bool canDashIntoClimb;
    [SerializeField] private bool canStopClimbWithHorizontalInput;

    private const string CLIMB_ANIM_PARAMETER_NAME = "climb";

    private float minClimbTime;
    private int parameterID;
    private bool isClimbing;

    private void OnEnable()
    {
        climbActionRef.action.performed += TryToClimb;
        climbActionRef.action.canceled += StopClimbing;
    }

    private void OnDisable()
    {
        climbActionRef.action.performed -= TryToClimb;
        climbActionRef.action.canceled -= StopClimbing;
    }

    protected override void Init()
    {
        base.Init();
        parameterID = Animator.StringToHash(CLIMB_ANIM_PARAMETER_NAME);
        minClimbTime = setMinClimbTime;
    }

    public override void EnterAbility()
    {
        linkedPhysics.hasDashReset = true;
    }

    public override void ProcessAbility()
    {
        if (isClimbing)
            minClimbTime -= Time.deltaTime;

        CanStopClimbWithHorizontalInput();

        if (!canClimb && !linkedPhysics.isGrounded)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.rb.linearVelocityY = 0f; // If you don't want upwards momentum when reaching the top of a climb
            return;
        }

        if (linkedPhysics.isGrounded && minClimbTime <= 0f)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            return;
        }
    }

    public override void ProcessFixedAbility()
    {
        if (isClimbing)
            linkedPhysics.rb.linearVelocity = new(0f, linkedInput.verticalInput * climbSpeed);
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(parameterID, linkedStateMachine.currentState == PlayerStates.State.Climb);
    }

    public override void ExitAbility()
    {
        linkedPhysics.EnableGravity();
        isClimbing = false;
        linkedAnimator.enabled = true;
    }

    private void TryToClimb(InputAction.CallbackContext context)
    {
        if (!isPermitted) return;
        linkedInput.verticalInput = climbActionRef.action.ReadValue<float>(); // This is here because the value isn't updated before this function, even with Script Execution Order set
        if (linkedPhysics.isGrounded && linkedInput.verticalInput < 0f)
        {
            print("Hey");
            return;
        }

        linkedAnimator.enabled = true;

        if (linkedStateMachine.currentState == PlayerStates.State.Climb
            || CanDashIntoClimb() //Optional
            || !canClimb)
            return;
        
        linkedStateMachine.ChangeState(PlayerStates.State.Climb);
        linkedPhysics.DisableGravity();
        linkedPhysics.ResetVelocity();
        isClimbing = true;
        minClimbTime = setMinClimbTime;
    }

    private void StopClimbing(InputAction.CallbackContext context)
    {
        if (!isPermitted) return;
        if (linkedStateMachine.currentState != PlayerStates.State.Climb) return;
        linkedPhysics.ResetVelocity();
        linkedAnimator.enabled = false;
    }

    private bool CanDashIntoClimb()
    {
        if (canDashIntoClimb) return false;

        return linkedStateMachine.currentState == PlayerStates.State.Dash;
    }

    private bool CanStopClimbWithHorizontalInput()
    {
        if (!canStopClimbWithHorizontalInput) return false;
        if (linkedInput.horizontalInput == 0f) return false;

        linkedStateMachine.ChangeState(PlayerStates.State.Jump);
        linkedPhysics.rb.linearVelocityY = 0f;

        return true;
    }
}