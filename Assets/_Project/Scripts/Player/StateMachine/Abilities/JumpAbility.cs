using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GatherInput))]
public class JumpAbility : BaseAbility
{
    public InputActionReference jumpActionRef;

    [SerializeField] private float jumpForce;
    [SerializeField] private float airSpeed;
    [SerializeField] private float minAirTime;

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
        }
    }

    public override void ProcessFixedAbility()
    {
        if (!linkedPhysics.isGrounded)
        {
            linkedPhysics.rb.linearVelocity = new(airSpeed * linkedInput.horizontalInput, linkedPhysics.rb.linearVelocityY);
        }
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(jumpParameterID, linkedStateMachine.currentState == PlayerStates.State.Jump);
        linkedAnimator.SetFloat(ySpeedParameterID, linkedPhysics.rb.linearVelocityY);
    }

    private void TryToJump(InputAction.CallbackContext context)
    {
        if (!isPermitted) return;

        if (linkedPhysics.isGrounded)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
            linkedPhysics.rb.linearVelocity = new(airSpeed * linkedInput.horizontalInput, jumpForce);
            minAirTime = startMinAirTime;
        }
    }

    private void StopJump(InputAction.CallbackContext context)
    {
        print("Stop Jumping");
    }
}