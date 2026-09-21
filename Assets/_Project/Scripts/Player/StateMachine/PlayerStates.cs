using System.Collections.Generic;
using UnityEngine;

public class PlayerStates
{
    public enum State
    {
        Idle,
        Run,
        Jump,
        DoubleJump,
        WallJump,
        WallSlide,
        Dash,
        Crouch,
        Ladders,
        None
    }
}