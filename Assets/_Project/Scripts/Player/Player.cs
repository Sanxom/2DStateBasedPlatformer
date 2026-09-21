using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GatherInput gatherInput;
    public StateMachine stateMachine;
    public PhysicsControl physicsControl;
    public Animator animator;
    public bool isFacingRight = true;

    private BaseAbility[] playerAbilities;

    private void Awake()
    {
        stateMachine = new();
        playerAbilities = GetComponents<BaseAbility>();
        stateMachine.abilityArray = playerAbilities;
    }

    private void Update()
    {
        foreach (BaseAbility ability in playerAbilities)
        {
            if (ability.abilityState == stateMachine.currentState)
            {
                ability.ProcessAbility();
            }

            ability.UpdateAnimator();
        }

        print($"Current state: {stateMachine.currentState}");
    }

    private void FixedUpdate()
    {
        foreach (BaseAbility ability in playerAbilities)
        {
            if (ability.abilityState == stateMachine.currentState)
            {
                ability.ProcessFixedAbility();
            }
        }
    }

    public void Flip()
    {
        if (isFacingRight && gatherInput.horizontalInput < 0f)
        {
            transform.Rotate(0f, 180f, 0f);
            isFacingRight = !isFacingRight;
        }
        else if (!isFacingRight && gatherInput.horizontalInput > 0f)
        {
            transform.Rotate(0f, 180f, 0f);
            isFacingRight = !isFacingRight;
        }
    }
}