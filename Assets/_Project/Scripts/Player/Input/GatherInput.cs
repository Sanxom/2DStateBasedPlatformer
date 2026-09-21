using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GatherInput : MonoBehaviour
{
    public PlayerInput playerInput;
    public InputActionReference moveActionRef;
    [HideInInspector] public float horizontalInput;

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
    }

    private void OnDisable()
    {
        playerMap.Disable();
        uiMap.Disable();
    }
}