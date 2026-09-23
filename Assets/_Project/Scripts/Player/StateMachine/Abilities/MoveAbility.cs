using System.Collections.Generic;
using UnityEngine;

public class MoveAbility : BaseAbility
{
    [SerializeField] private float speed;

    private const string RUN_ANIM_PARAMETER_NAME = "run";
    private int runParameterID;

    protected override void Init()
    {
        base.Init();
        runParameterID = Animator.StringToHash(RUN_ANIM_PARAMETER_NAME);
    }

    public override void EnterAbility()
    {
        player.Flip();

        if (linkedPhysics.isGrounded)
            linkedPhysics.hasDashReset = true;
    }

    public override void ProcessAbility()
    {
        if (linkedPhysics.isGrounded && linkedInput.horizontalInput == 0)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Idle);
        }

        if (!linkedPhysics.isGrounded)
        {
            linkedStateMachine.ChangeState(PlayerStates.State.Jump);
        }
    }

    public override void ProcessFixedAbility()
    {
        linkedPhysics.rb.linearVelocity = new(speed * linkedInput.horizontalInput, linkedPhysics.rb.linearVelocityY);
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(runParameterID, linkedStateMachine.currentState == PlayerStates.State.Run);
    }
}