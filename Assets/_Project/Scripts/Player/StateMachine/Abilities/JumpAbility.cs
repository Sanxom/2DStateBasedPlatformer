using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GatherInput))]
public class JumpAbility : BaseAbility
{
    public InputActionReference jumpActionRef;

    [SerializeField] private float jumpForce;
    [SerializeField] private float airSpeed;
    [SerializeField] private float minAirTime;
    [SerializeField] private bool canJumpFromCrouch;

    private float startMinAirTime;
    private int jumpParameterID;
    private int ySpeedParameterID;

    private const string JUMP_ANIM_PARAMETER_NAME = "jump";
    private const string Y_SPEED_ANIM_PARAMETER_NAME = "ySpeed";

    private void OnEnable()
    {
        jumpActionRef.action.performed += TryToJump;
        jumpActionRef.action.canceled += StopJump;
    }

    private void OnDisable()
    {
        jumpActionRef.action.performed -= TryToJump;
        jumpActionRef.action.canceled -= StopJump;
    }

    protected override void Init()
    {
        base.Init();
        startMinAirTime = minAirTime;
        jumpParameterID = Animator.StringToHash(JUMP_ANIM_PARAMETER_NAME);
        ySpeedParameterID = Animator.StringToHash(Y_SPEED_ANIM_PARAMETER_NAME);
    }

    public override void ProcessAbility()
    {
        player.Flip();

        minAirTime -= Time.deltaTime;

        if (linkedPhysics.isGrounded && minAirTime < 0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            return;
        }

        if (!linkedPhysics.isGrounded && linkedPhysics.isWallDetectedUpper && linkedPhysics.rb.linearVelocityY <= 0f)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.WallSlide);
            return;
        }
    }

    public override void ProcessFixedAbility()
    {
        if (linkedInput.horizontalInput != 0f)
        {
            linkedPhysics.rb.linearVelocity = new(airSpeed * linkedInput.horizontalInput, linkedPhysics.rb.linearVelocityY);
            return;
        }

        if (!linkedPhysics.isGrounded && linkedPhysics.didWallJump)
        {
            linkedPhysics.rb.linearVelocity = new(linkedPhysics.rb.linearVelocityX, linkedPhysics.rb.linearVelocityY);
            return;
        }
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(jumpParameterID, linkedStateMachine.currentState == PlayerStates.State.Jump || linkedStateMachine.currentState == PlayerStates.State.WallJump);
        linkedAnimator.SetFloat(ySpeedParameterID, linkedPhysics.rb.linearVelocityY);
    }

    private void TryToJump(InputAction.CallbackContext context)
    {
        if (!isPermitted
            || !CanJumpFromCrouch()) return;

        if (linkedStateMachine.currentState == PlayerStates.State.Climb)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.rb.linearVelocity = new(airSpeed * linkedInput.horizontalInput, 0f);
            minAirTime = startMinAirTime;
            return;
        }

        if (linkedPhysics.isGrounded)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.rb.linearVelocity = new(airSpeed * linkedInput.horizontalInput, jumpForce);
            minAirTime = startMinAirTime;
            return;
        }
    }

    private void StopJump(InputAction.CallbackContext context)
    {
        if (!isPermitted) return;
        if (linkedPhysics.isGrounded) return;

        if (linkedPhysics.rb.linearVelocityY > 0f)
        {
            linkedPhysics.rb.linearVelocity = new(linkedPhysics.rb.linearVelocityX, 0.1f);
        }
    }

    private bool CanJumpFromCrouch()
    {
        if (!canJumpFromCrouch && linkedStateMachine.currentState == PlayerStates.State.Crouch)
            return false;
        else if (canJumpFromCrouch && linkedStateMachine.currentState == PlayerStates.State.Crouch)
            return true;
        return true;
    }
}