using UnityEngine;

[RequireComponent(typeof(Player))]
public class BaseAbility : MonoBehaviour
{
    public PlayerStates.State abilityState;
    public bool isPermitted = true;

    protected Player player;
    protected GatherInput linkedInput;
    protected StateMachine linkedStateMachine;
    protected PhysicsControl linkedPhysics;
    protected Animator linkedAnimator;

    protected virtual void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        player = GetComponent<Player>();

        if (player != null)
        {
            linkedInput = player.gatherInput;
            linkedStateMachine = player.stateMachine;
            linkedAnimator = player.animator;
            linkedPhysics = player.physicsControl;
        }
    }

    public virtual void EnterAbility()
    {

    }

    public virtual void ExitAbility()
    {

    }

    public virtual void ProcessAbility()
    {

    }

    public virtual void ProcessFixedAbility()
    {

    }

    public virtual void UpdateAnimator()
    {

    }
}