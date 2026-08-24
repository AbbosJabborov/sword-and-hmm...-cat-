using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// Global game state management
    /// Tracks whether player is in Gameplay mode or UI mode (Journal, Menus, etc)
    /// Other systems check this before processing input
    /// </summary>
    public enum GameState
    {
        Gameplay,  // Normal exploration, gathering, cooking
        Journal,   // Journal UI is open
        Paused,    // Future: Pause menu (can extend later)
        Dialog     // Future: NPC conversation (can extend later)
    }
    
    public class GameStateManager : MonoBehaviour
    {
        private static GameStateManager instance;
        private GameState currentState = GameState.Gameplay;
        
        public static GameStateManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<GameStateManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject("GameStateManager");
                        instance = obj.AddComponent<GameStateManager>();
                    }
                }
                return instance;
            }
        }
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }
        
        /// <summary>
        /// Get current game state
        /// </summary>
        public static GameState GetState()
        {
            return Instance.currentState;
        }
        
        /// <summary>
        /// Check if player is in gameplay mode (can use F, R, W, etc)
        /// </summary>
        public static bool IsGameplayActive()
        {
            return Instance.currentState == GameState.Gameplay;
        }
        
        /// <summary>
        /// Check if journal UI is open
        /// </summary>
        public static bool IsJournalOpen()
        {
            return Instance.currentState == GameState.Journal;
        }
        
        /// <summary>
        /// Set game state
        /// </summary>
        public static void SetState(GameState newState)
        {
            GameStateManager instance = Instance;
            if (instance.currentState != newState)
            {
                instance.currentState = newState;
                Debug.Log($"[STATE] Changed to: {newState}");
            }
        }
        
        /// <summary>
        /// Toggle between Gameplay and Journal
        /// </summary>
        public static void ToggleJournalState()
        {
            GameState newState = Instance.currentState == GameState.Journal 
                ? GameState.Gameplay 
                : GameState.Journal;
            SetState(newState);
        }
    }
}