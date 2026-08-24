using Game.Cooking;
using Game.Interaction;
using Game.Player;
using Game.Core;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Player
{
    /// <summary>
    /// Handles all player input (movement, interaction, consumption)
    /// 
    /// Input actions from InputSystem:
    /// - Move (WASD / Left Stick)
    /// - Run (Shift / LB)
    /// - Jump (Space / A button)
    /// - Interact (F / X button)
    /// - Consume (R / RT)
    /// - CycleLeft (Q / L1) - Used for hotbar OR journal nav
    /// - CycleRight (E / R1) - Used for hotbar OR journal nav
    /// - OpenJournal (I / Y button)
    /// 
    /// When journal is open:
    /// - Only OpenJournal, CycleLeft, CycleRight work
    /// - All gameplay inputs (Move, Interact, Consume, etc) are blocked
    /// 
    /// When gameplay is active:
    /// - All inputs work normally
    /// </summary>
    public class PlayerInputHandler : MonoBehaviour
    {
        private PlayerMovement movement;
        private HotbarUI hotbar;
        private Interact interact;
        private CookingMinigame cookingMinigame;
        private JournalBook journalBook;
        
        private void Awake()
        {
            movement = GetComponent<PlayerMovement>();
            hotbar = FindFirstObjectByType<HotbarUI>();
            interact = GetComponent<Interact>();
            cookingMinigame = FindFirstObjectByType<CookingMinigame>();
            journalBook = FindFirstObjectByType<JournalBook>();
        }
        
        // ==================== MOVEMENT INPUT ====================
        
        public void OnMove(InputAction.CallbackContext context)
        {
            // Movement always works, even with journal open
            // (player should be able to look around)
            // If you want to disable movement too, add this check:
            // if (!GameStateManager.IsGameplayActive()) return;
            
            movement.SetMoveInput(context.ReadValue<Vector2>());
        }
        
        public void OnRun(InputAction.CallbackContext context)
        {
            // Running only works during gameplay
            if (!GameStateManager.IsGameplayActive()) return;
            
            movement.SetRunning(context.ReadValueAsButton());
        }
        
        public void OnJump(InputAction.CallbackContext context)
        {
            // Jumping only works during gameplay
            if (!GameStateManager.IsGameplayActive()) return;
            
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
        
        // ==================== JOURNAL INPUT ====================
        
        public void OnOpenJournal(InputAction.CallbackContext context)
        {
            // Journal can be opened/closed anytime
            if (context.performed)
            {
                Debug.Log("[INPUT] I pressed (Toggle Journal)");
                journalBook.ToggleJournal();
                GameStateManager.ToggleJournalState();
            }
        }
        
        // ==================== HOTBAR / JOURNAL NAVIGATION ====================
        
        /// <summary>
        /// Q / L1 button
        /// During gameplay: Cycles hotbar left
        /// During journal: Navigates to previous page
        /// </summary>
        public void OnCycleLeft(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            if (GameStateManager.IsJournalOpen())
            {
                // Journal is open: navigate previous page
                Debug.Log("[INPUT] Q pressed (Journal: Previous Page)");
                journalBook.RotateBack();
            }
            else if (GameStateManager.IsGameplayActive())
            {
                // Gameplay: cycle hotbar left
                if (hotbar)
                {
                    Debug.Log("[INPUT] Q pressed (Hotbar: Cycle Left)");
                    hotbar.OnCycleLeft();
                }
            }
        }
        
        /// <summary>
        /// E / R1 button
        /// During gameplay: Cycles hotbar right
        /// During journal: Navigates to next page
        /// </summary>
        public void OnCycleRight(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            if (GameStateManager.IsJournalOpen())
            {
                // Journal is open: navigate next page
                Debug.Log("[INPUT] E pressed (Journal: Next Page)");
                journalBook.RotateForward();
            }
            else if (GameStateManager.IsGameplayActive())
            {
                // Gameplay: cycle hotbar right
                if (hotbar)
                {
                    Debug.Log("[INPUT] E pressed (Hotbar: Cycle Right)");
                    hotbar.OnCycleRight();
                }
            }
        }
        
        // ==================== INTERACTION INPUT ====================
        
        public void OnInteract(InputAction.CallbackContext context)
        {
            // Interact only works during gameplay
            if (!GameStateManager.IsGameplayActive()) return;
            
            if (context.performed)
            {
                Debug.Log("[INPUT] F pressed (Interact)");
                interact.OnInteract(gameObject);
            }
        }
        
        public void OnConsume(InputAction.CallbackContext context)
        {
            // Consume only works during gameplay
            if (!GameStateManager.IsGameplayActive()) return;
            
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
    }
}