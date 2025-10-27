using Game.Player.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerMovement _movement;
        private PlayerLook _look;

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>();
            _look = GetComponent<PlayerLook>();
        }

        public void OnMove(InputAction.CallbackContext context)
            => _movement.SetMoveInput(context.ReadValue<Vector2>());

        public void OnRun(InputAction.CallbackContext context)
            => _movement.SetRunning(context.ReadValueAsButton());

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                _movement.Jump();
        }

        public void OnLook(InputAction.CallbackContext context)
            => _look.SetLookInput(context.ReadValue<Vector2>());
    }
}