using UnityEngine;
using UnityEngine.InputSystem;

public class MultipleJumpAbility :JumpAbility
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float airSpeed;
    [SerializeField] private float minAirTime;
    [SerializeField] private bool canJumpFromCrouch;
    [SerializeField] private bool multiJumpPermitted;

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
        numJumps = maxNumJumps;
        jumpParameterID = Animator.StringToHash(JUMP_ANIM_PARAMETER_NAME);
        ySpeedParameterID = Animator.StringToHash(Y_SPEED_ANIM_PARAMETER_NAME);
    }

    public override void ProcessAbility()
    {
        minAirTime -= Time.deltaTime;

        if (linkedPhysics.isGrounded && minAirTime < 0)
        {
            linkedPhysics.isInTheAir = false;
            numJumps = maxNumJumps;
            if (linkedInput.horizontalInput != 0f)
                linkedStateMachine.ChangeState(PlayerStates.State.Run);
            else
                linkedStateMachine.ChangeState(PlayerStates.State.Idle);
            return;
        }

        if (!linkedPhysics.isGrounded && linkedPhysics.isWallDetectedUpper && linkedPhysics.rb.linearVelocityY <= 0f)
        {
            linkedPhysics.isInTheAir = false;
            numJumps = maxNumJumps;
            linkedStateMachine.ChangeState(PlayerStates.State.WallSlide);
            return;
        }

        player.Flip();
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

    public void SetMaxJumpNumber(int maxJumps)
    {
        maxNumJumps = maxJumps;
    }

    private void TryToJump(InputAction.CallbackContext context)
    {
        if (!isPermitted 
            || linkedStateMachine.currentState == PlayerStates.State.Dash
            || linkedStateMachine.currentState == PlayerStates.State.Knockback) 
            return;

        if (linkedStateMachine.currentState == PlayerStates.State.Climb)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.isInTheAir = true;
            numJumps = maxNumJumps;
            canActivateAdditionalJumps = true;
            UseOneJump(airSpeed * linkedInput.horizontalInput, 0f);
            return;
        }

        if (linkedPhysics.coyoteTimer > 0f)
        {
            if (linkedPhysics.isInTheAir) return;
            if (!CanJumpFromCrouch()) return;

            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.isInTheAir = true;
            numJumps = maxNumJumps;
            canActivateAdditionalJumps = true;
            UseOneJump(airSpeed * linkedInput.horizontalInput, jumpForce);
            return;
        }

        if (numJumps > 0 && canActivateAdditionalJumps)
        {
            if (!multiJumpPermitted) return;

            linkedPhysics.isInTheAir = true;
            UseOneJump(airSpeed * linkedInput.horizontalInput, jumpForce);
        }
        else
            canActivateAdditionalJumps = false;
    }

    private void StopJump(InputAction.CallbackContext context)
    {
        if (!isPermitted) return;
        if (linkedPhysics.isGrounded) return;

        if (linkedPhysics.rb.linearVelocityY > 0f)
        {
            linkedPhysics.rb.linearVelocity = new(linkedPhysics.rb.linearVelocityX, -0.1f);
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

    private void UseOneJump(float xValue, float jumpForce)
    {
        linkedPhysics.rb.linearVelocity = new(xValue, jumpForce);
        minAirTime = startMinAirTime;
        linkedPhysics.coyoteTimer = -1f;
        numJumps--;
    }
}