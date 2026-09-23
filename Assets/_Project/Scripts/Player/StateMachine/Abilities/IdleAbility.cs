using System.Collections.Generic;
using UnityEngine;

public class IdleAbility : BaseAbility
{
    private const string IDLE_ANIM_PARAMETER_NAME = "idle";
    private int idleParameterID;

    protected override void Init()
    {
        base.Init();
        idleParameterID = Animator.StringToHash(IDLE_ANIM_PARAMETER_NAME);
    }

    public override void EnterAbility()
    {
        linkedPhysics.rb.linearVelocityX = 0f;
        linkedPhysics.didWallJump = false;
    }

    public override void ProcessAbility()
    {
        if (linkedInput.horizontalInput != 0)
        {
            player.Flip();
            linkedStateMachine.ChangeState(PlayerStates.State.Run);
        }
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(idleParameterID, linkedStateMachine.currentState == PlayerStates.State.Idle);
    }
}