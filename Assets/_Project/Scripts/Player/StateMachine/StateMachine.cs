[System.Serializable]
public class StateMachine
{
    public PlayerStates.State previousState;
    public PlayerStates.State currentState;
    public BaseAbility[] abilityArray;

    public void ChangeState(PlayerStates.State newState)
    {
        foreach (BaseAbility ability in abilityArray)
        {
            if (ability.abilityState == newState)
                if (!ability.isPermitted)
                    return;
        }

        foreach (BaseAbility ability in abilityArray)
        {
            if (ability.abilityState == currentState)
            {
                ability.ExitAbility();
                previousState = currentState;
            }
        }

        foreach (BaseAbility ability in abilityArray)
        {
            if (ability.abilityState == newState)
            {
                if (ability.isPermitted)
                {
                    currentState = newState;
                    ability.EnterAbility();
                }
                break;
            }
        }
    }

    public void ForceChange(PlayerStates.State newState)
    {
        previousState = currentState;
        currentState = newState;
    }
}