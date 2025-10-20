using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerLook look;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();
    }

    public void OnMove(InputAction.CallbackContext context)
        => movement.SetMoveInput(context.ReadValue<Vector2>());

    public void OnRun(InputAction.CallbackContext context)
        => movement.SetRunning(context.ReadValueAsButton());

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            movement.Jump();
    }

    public void OnLook(InputAction.CallbackContext context)
        => look.SetLookInput(context.ReadValue<Vector2>());
}