using Game.Player.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerMovement movement;

        private void Awake()
        {
            movement = GetComponent<PlayerMovement>();
        }

        public void OnMove(InputAction.CallbackContext context)
            => movement.SetMoveInput(context.ReadValue<Vector2>());

        public void OnRun(InputAction.CallbackContext context)
            => movement.SetRunning(context.ReadValueAsButton());
    }
}