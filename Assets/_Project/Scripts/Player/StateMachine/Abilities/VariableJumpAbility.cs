using UnityEngine;
using UnityEngine.InputSystem;

public class VariableJumpAbility : BaseAbility
{
    public InputActionReference jumpActionRef;

    [SerializeField] private float jumpForce;
    [SerializeField] private float airSpeed;
    [SerializeField] private float minAirTime;
    [SerializeField] private float maxJumpTime;
    [SerializeField] private float gravityDivider;
    [SerializeField] private bool canJumpFromCrouch;

    private float startMinAirTime;
    private float jumpTimer;
    private int jumpParameterID;
    private int ySpeedParameterID;
    private bool isJumping;

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
        minAirTime -= Time.deltaTime;

        if (isJumping)
        {
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f)
            {
                isJumping = false;
            }
        }


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
        player.Flip();
    }

    public override void ProcessFixedAbility()
    {
        //if (linkedInput.horizontalInput != 0f)
        //{
        //    linkedPhysics.rb.linearVelocity = new(airSpeed * linkedInput.horizontalInput, linkedPhysics.rb.linearVelocityY);
        //    return;
        //}

        if (!linkedPhysics.isGrounded && linkedPhysics.didWallJump)
        {
            linkedPhysics.rb.linearVelocity = new(linkedPhysics.rb.linearVelocityX, linkedPhysics.rb.linearVelocityY);
            return;
        }
        if (!linkedPhysics.isGrounded)
        {
            if (isJumping)
                linkedPhysics.rb.linearVelocity = new(airSpeed * linkedInput.horizontalInput, jumpForce);
            else
                linkedPhysics.rb.linearVelocity = new(airSpeed * linkedInput.horizontalInput, Mathf.Clamp(linkedPhysics.rb.linearVelocityY, -10f, jumpForce));
        }

        if (linkedPhysics.rb.linearVelocityY < 0f)
            linkedPhysics.rb.gravityScale = linkedPhysics.GetGravity() / gravityDivider;
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(jumpParameterID, linkedStateMachine.currentState == PlayerStates.State.Jump || linkedStateMachine.currentState == PlayerStates.State.WallJump);
        linkedAnimator.SetFloat(ySpeedParameterID, linkedPhysics.rb.linearVelocityY);
    }

    public override void ExitAbility()
    {
        linkedPhysics.EnableGravity();
    }

    private void TryToJump(InputAction.CallbackContext context)
    {
        if (!isPermitted
            || linkedStateMachine.currentState == PlayerStates.State.Dash
            || linkedStateMachine.currentState == PlayerStates.State.Knockback) return;

        if (linkedStateMachine.currentState == PlayerStates.State.Climb)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.rb.linearVelocity = new(airSpeed * linkedInput.horizontalInput, 0f);
            minAirTime = startMinAirTime;
            isJumping = true;
            jumpTimer = maxJumpTime;
            return;
        }

        if (linkedPhysics.coyoteTimer > 0f)
        {
            if (!CanJumpFromCrouch()) return;

            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.rb.linearVelocity = new(airSpeed * linkedInput.horizontalInput, jumpForce);
            minAirTime = startMinAirTime;
            linkedPhysics.coyoteTimer = -1f;
            isJumping = true;
            jumpTimer = maxJumpTime;
            return;
        }
    }

    private void StopJump(InputAction.CallbackContext context)
    {
        isJumping = false;
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