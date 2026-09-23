using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private ClimbAbility climbAbility;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        climbAbility = collision.GetComponent<ClimbAbility>();
        if (climbAbility != null && climbAbility.isPermitted)
        {
            climbAbility.canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (climbAbility != null && climbAbility.isPermitted)
            climbAbility.canClimb = false;
    }
}