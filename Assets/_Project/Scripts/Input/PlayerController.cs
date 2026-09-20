using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GameInput gameInput; 
    private InputActionMap playerMap;
    private InputActionMap uiMap;
    private InputAction moveAction;
    private InputAction jumpAction;
    private float horizontalValue;

    private void Awake()
    {
        gameInput = new();
        playerMap = gameInput.Player;
        uiMap = gameInput.UI;
        moveAction = gameInput.Player.Move;
        jumpAction = gameInput.Player.Jump;
    }

    private void OnEnable()
    {
        jumpAction.performed += TryToJump;
        jumpAction.canceled += StopJump;

        playerMap.Enable();
        uiMap.Disable();
    }

    private void Update()
    {
        horizontalValue = moveAction.ReadValue<float>();
        print(horizontalValue);
    }

    private void OnDisable()
    {
        jumpAction.performed -= TryToJump;
        jumpAction.canceled -= StopJump;
        gameInput.Disable();
    }

    private void TryToJump(InputAction.CallbackContext context)
    {
        print("Jumped");
    }

    private void StopJump(InputAction.CallbackContext context)
    {
        print("Stop Jumping");
    }
}