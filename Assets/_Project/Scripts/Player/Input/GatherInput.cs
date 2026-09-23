using UnityEngine;
using UnityEngine.InputSystem;

public class GatherInput : MonoBehaviour
{
    public PlayerInput playerInput;
    public InputActionReference moveActionRef;
    public InputActionReference climbActionRef;
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;

    private InputActionMap playerMap;
    private InputActionMap uiMap;

    private void Start()
    {
        playerMap = playerInput.actions.FindActionMap("Player");
        uiMap = playerInput.actions.FindActionMap("UI");

        playerMap.Enable();
        uiMap.Disable();
    }

    private void Update()
    {
        horizontalInput = moveActionRef.action.ReadValue<float>();
        verticalInput = climbActionRef.action.ReadValue<float>();
    }

    private void OnDisable()
    {
        playerMap.Disable();
        uiMap.Disable();
    }

    public void DisablePlayerMap()
    {
        playerMap.Disable();
    }
}