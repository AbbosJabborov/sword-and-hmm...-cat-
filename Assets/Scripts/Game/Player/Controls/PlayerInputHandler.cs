using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Game.Player.Controls
{
    [RequireComponent(typeof(PlayerReferences))]
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerMovement movement;
        private PlayerLook look;
        
        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }

		private void Awake()
		{
		    var refs = GetComponent<PlayerReferences>();
    		movement = GetComponent<PlayerMovement>();
   			look = GetComponent<PlayerLook>();
		}
        
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            movement?.SetMoveInput(MoveInput);
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            bool isRunning = context.ReadValueAsButton();
            movement?.SetRunning(isRunning);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                movement?.Jump();
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.performed)
                movement?.ToggleCrouch();
        }

        public void OnSlide(InputAction.CallbackContext context)
        {
            if (context.performed)
                movement?.Slide();
        }

        // --- Look ---
        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
            look?.SetLookInput(LookInput);
        }
    }
}