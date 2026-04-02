using Game.Cooking;
using Game.Inventory;
using Game.Systems;
using UI;
using UnityEngine;

namespace Game.Interaction
{
    public class CookingStation : MonoBehaviour, IInteractable
    {
        [SerializeField] private CookingMinigame cookingMinigame;
        [SerializeField] private AudioClip cookingStartSound;

        private HotbarUI _hotbar;
        private PlayerInventory _inventory;
        private HungerSystem _hungerSystem;
        private ItemDatabase _itemDatabase;
        private Game.Systems.AirQualitySystem _airQualitySystem;

        private void Start()
        {
            _hotbar = FindFirstObjectByType<HotbarUI>();
            _inventory = FindFirstObjectByType<PlayerInventory>();
            _hungerSystem = FindFirstObjectByType<HungerSystem>();
            _itemDatabase = FindFirstObjectByType<ItemDatabase>();
            _airQualitySystem = FindFirstObjectByType<Game.Systems.AirQualitySystem>();
            
        }

        public void Interact(GameObject interactor)
        {
            if (!cookingMinigame || cookingMinigame.IsActive)
            {
                Debug.Log("[COOKING] Minigame already active or not assigned!");
                return;
            }

            // Get selected food from hotbar
            string selectedFood = _hotbar.GetCurrentSelectedItem();
            int quantity = _hotbar.GetCurrentSelectedQuantity();

            if (string.IsNullOrEmpty(selectedFood) || quantity == 0)
            {
                Debug.Log("[COOKING] No food selected!");
                return;
            }

            // Check if food is cookable
            if (!IsCookable(selectedFood))
            {
                Debug.Log($"[COOKING] {selectedFood} is not cookable!");
                return;
            }

            // Get energy from ItemDatabase
            float energy = _hotbar.GetFoodEnergy(selectedFood);
            if (energy <= 0)
            {
                Debug.Log($"[COOKING] {selectedFood} has no energy value!");
                return;
            }

            Debug.Log($"[COOKING] Starting minigame for {selectedFood} (base energy: {energy})");

            // Start cooking minigame
            cookingMinigame.StartCooking(selectedFood, energy, _inventory, _hungerSystem, _airQualitySystem);

            // Play sound
            if (cookingStartSound)
                AudioSource.PlayClipAtPoint(cookingStartSound, transform.position);
        }

        private bool IsCookable(string foodName)
        {
            return foodName switch
            {
                "berry" => true,
                "mushroom" => true,
                _ => false
            };
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 1.5f);
        }
    }
}