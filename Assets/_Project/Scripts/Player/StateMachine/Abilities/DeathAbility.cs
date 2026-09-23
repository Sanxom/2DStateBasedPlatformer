using UnityEngine;

public class DeathAbility : BaseAbility
{
    private const string DEATH_ANIM_PARAMETER_NAME = "death";

    private int deathParameterID;
    private bool isGrounded;

    protected override void Init()
    {
        base.Init();

        deathParameterID = Animator.StringToHash(DEATH_ANIM_PARAMETER_NAME);
    }

    public override void EnterAbility()
    {
        player.gatherInput.DisablePlayerMap();
        linkedPhysics.ResetVelocity();

        //if (linkedPhysics.isGrounded)
        //    linkedAnimator.SetBool(deathParameterID, true);
        //else
        //{
        //    // Air death animation
        //    linkedAnimator.SetBool(deathParameterID, true);
        //}
    }

    public override void ProcessAbility()
    {
        if (isGrounded) return;

        if (linkedPhysics.isGrounded)
        {
            linkedAnimator.SetBool(deathParameterID, true);
            isGrounded = true;
        }
    }

    public void ResetGame()
    {
        print("Reset Game");
    }
}