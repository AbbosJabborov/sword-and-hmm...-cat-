using Game.Cooking;
using Game.Interaction;
using Game.Player;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerMovement movement;
        private HotbarUI hotbar;
        private Interact interact;
        private CookingMinigame cookingMinigame;

        private void Awake()
        {
            movement = GetComponent<PlayerMovement>();
            hotbar = FindFirstObjectByType<HotbarUI>();
            interact = GetComponent<Interact>();
            cookingMinigame = FindFirstObjectByType<CookingMinigame>();
        }

        public void OnMove(InputAction.CallbackContext context)
            => movement.SetMoveInput(context.ReadValue<Vector2>());

        public void OnRun(InputAction.CallbackContext context)
            => movement.SetRunning(context.ReadValueAsButton());

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // If cooking minigame is active, Space evaluates cooking instead of jumping
                if (cookingMinigame && cookingMinigame.IsActive)
                {
                    Debug.Log("[INPUT] Space pressed (Cooking Evaluation)");
                    cookingMinigame.OnSpacePressed();
                }
                else
                {
                    Debug.Log("[INPUT] Space pressed (Jump)");
                    movement.Jump();
                }
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("[INPUT] F pressed (Interact)");
                interact.OnInteract(gameObject);
            }
        }

        public void OnConsume(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log("[INPUT] R pressed (Consume)");
                interact.OnConsumePressed(gameObject);
            }
            else if (context.canceled)
            {
                Debug.Log("[INPUT] R released (Consume)");
                interact.OnConsumeReleased(gameObject);
            }
        }

        public void OnCycleLeft(InputAction.CallbackContext context)
        {
            if (context.performed && hotbar)
            {
                Debug.Log("[INPUT] Q pressed (Cycle Left)");
                hotbar.OnCycleLeft();
            }
        }

        public void OnCycleRight(InputAction.CallbackContext context)
        {
            if (context.performed && hotbar)
            {
                Debug.Log("[INPUT] E pressed (Cycle Right)");
                hotbar.OnCycleRight();
            }
        }
    }
}