using System.Collections;
using UnityEngine;

public class KnockbackAbility : BaseAbility
{
    private Coroutine currentKnockbackCoroutine;

    private const string KNOCKBACK_ANIM_PARAMETER_NAME = "knockback";

    private int knockbackParameterID;

    protected override void Init()
    {
        base.Init();

        knockbackParameterID = Animator.StringToHash(KNOCKBACK_ANIM_PARAMETER_NAME);
    }

    public override void UpdateAnimator()
    {
        linkedAnimator.SetBool(knockbackParameterID, linkedStateMachine.currentState == PlayerStates.State.Knockback);
    }

    public override void ExitAbility()
    {
        currentKnockbackCoroutine = null;
    }

    public IEnumerator KnockbackCoroutine(float duration, Vector2 force, Transform enemyObject)
    {
        linkedStateMachine.ChangeState(PlayerStates.State.Knockback);
        linkedPhysics.ResetVelocity();
        
        if (player.transform.position.x >= enemyObject.position.x)
            linkedPhysics.rb.linearVelocity = force;
        else
            linkedPhysics.rb.linearVelocity = new(-force.x, force.y);

        yield return new WaitForSeconds(duration);

        switch (player.playerStats.CurrentHealth)
        {
            case > 0f:
                if (linkedPhysics.isGrounded)
                {
                    linkedPhysics.isInTheAir = false;
                    if (linkedInput.horizontalInput != 0f)
                        linkedStateMachine.ChangeState(PlayerStates.State.Run);
                    else
                        linkedStateMachine.ChangeState(PlayerStates.State.Idle);
                }
                else
                {
                    linkedPhysics.isInTheAir = true;
                    linkedStateMachine.ChangeState(PlayerStates.State.Jump);
                }
                break;
            default:
                linkedStateMachine.ChangeState(PlayerStates.State.Dead);
                break;
        }
    }

    public IEnumerator SwingKnockbackCoroutine(float duration, Vector2 force, int direction)
    {
        linkedStateMachine.ChangeState(PlayerStates.State.Knockback);
        linkedPhysics.ResetVelocity();

        force.x *= direction;
        linkedPhysics.rb.linearVelocity = force;

        yield return new WaitForSeconds(duration);

        switch (player.playerStats.CurrentHealth)
        {
            case > 0f:
                if (linkedPhysics.isGrounded)
                {
                    linkedPhysics.isInTheAir = false;
                    if (linkedInput.horizontalInput != 0f)
                        linkedStateMachine.ChangeState(PlayerStates.State.Run);
                    else
                        linkedStateMachine.ChangeState(PlayerStates.State.Idle);
                }
                else
                {
                    linkedPhysics.isInTheAir = true;
                    linkedStateMachine.ChangeState(PlayerStates.State.Jump);
                }
                break;
            default:
                linkedStateMachine.ChangeState(PlayerStates.State.Dead);
                break;
        }
    }

    public void StartKnockback(float duration, Vector2 force, Transform enemyObject)
    {
        if (!player.playerStats.CanTakeDamage) return;

        if (currentKnockbackCoroutine == null)
        {
            currentKnockbackCoroutine = StartCoroutine(KnockbackCoroutine(duration, force, enemyObject));
        }
        else
        {
            // Do nothing OR vv
            StopCoroutine(currentKnockbackCoroutine);
            currentKnockbackCoroutine = StartCoroutine(KnockbackCoroutine(duration, force, enemyObject));
        }
    }

    public void StartSwingKnockback(float duration, Vector2 force, int direction)
    {
        if (!player.playerStats.CanTakeDamage) return;

        if (currentKnockbackCoroutine == null)
        {
            currentKnockbackCoroutine = StartCoroutine(SwingKnockbackCoroutine(duration, force, direction));
        }
        else
        {
            // Do nothing OR vv
            StopCoroutine(currentKnockbackCoroutine);
            currentKnockbackCoroutine = StartCoroutine(SwingKnockbackCoroutine(duration, force, direction));
        }
    }
}