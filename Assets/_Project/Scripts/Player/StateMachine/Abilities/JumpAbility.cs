using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GatherInput))]
public class JumpAbility : BaseAbility
{
    [SerializeField] protected InputActionReference jumpActionRef;
    [SerializeField] protected int maxNumJumps;
    [SerializeField] protected int numJumps;

    protected bool canActivateAdditionalJumps;
}