using UnityEngine;
using UnityEngine.InputSystem;

public class CrouchAbility : BaseAbility
{
    // TODO: Add crouchRun animation sprites and remove the color change; right now, we're just changing the color to red in the Animation tab
    [SerializeField] private InputActionReference crouchActionRef;
    [SerializeField] private float crouchSpeed;

    private const string CROUCH_ANIM_PARAMETER_NAME = "crouch";
    private const string X_SPEED_ANIM_PARAMETER_NAME = "xSpeed";

    private int crouchParameterID;
    private int xSpeedParameterID;
    private bool shouldStop;

    private void OnEnable()
    {
        crouchActionRef.action.performed += TryToCrouch;
        crouchActionRef.action.canceled += StopCrouch;
    }

    private void OnDisable()
    {
        crouchActionRef.action.performed -= TryToCrouch;
        crouchActionRef.action.canceled -= StopCrouch;
    }

    protected override void Init()
    {
        base.Init();

        crouchParameterID = Animator.StringToHash(CROUCH_ANIM_PARAMETER_NAME);
        xSpeedParameterID = Animator.StringToHash(X_SPEED_ANIM_PARAMETER_NAME);
    }

    public override void EnterAbility()
    {
        linkedPhysics.EnableCrouchCollider();
        player.playerStats.EnableStatsCrouchCollider();
    }

    public override void ProcessAbility()
    {
        player.Flip();

        if (shouldStop && !linkedPhysics.isCeilingDetected)
        {
            ToggleCrouchOff();
        }

        if (!linkedPhysics.isGrounded)
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
    }

    public override void ProcessFixedAbility()
    {
        if (linkedPhysics.isGrounded)
            linkedPhysics.rb.linearVelocity = new(linkedInput.horizontalInput * crouchSpeed, linkedPhysics.rb.linearVelocityY);
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(crouchParameterID, linkedStateMachine.currentState == PlayerStates.State.Crouch);
        linkedAnimator.SetFloat(xSpeedParameterID, Mathf.Abs(linkedPhysics.rb.linearVelocityX));
    }

    public override void ExitAbility()
    {
        shouldStop = false;
        linkedPhysics.EnableStandCollider();
        player.playerStats.EnableStatsStandCollider();
    }

    private void TryToCrouch(InputAction.CallbackContext context)
    {
        if (!isPermitted 
            || !linkedPhysics.isGrounded
            || linkedStateMachine.currentState == PlayerStates.State.Dash
            || linkedStateMachine.currentState == PlayerStates.State.Climb
            || linkedStateMachine.currentState == PlayerStates.State.Knockback) 
            return;

        shouldStop = false;
        linkedStateMachine.ChangeState(PlayerStates.State.Crouch);
    }

    private void StopCrouch(InputAction.CallbackContext context)
    {
        if (!isPermitted 
            || linkedStateMachine.currentState != PlayerStates.State.Crouch) 
            return;

        if (linkedPhysics.isCeilingDetected)
        {
            shouldStop = true;
            return;
        }

        ToggleCrouchOff();
    }

    private void ToggleCrouchOff()
    {
        if (linkedInput.horizontalInput == 0f)
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
        else if (linkedInput.horizontalInput != 0f)
            linkedStateMachine.ChangeState(PlayerStates.State.Run);
    }
}