using Game.Cooking;
using Game.Inventory;
using Game.Systems;
using UI;
using TMPro;
using UnityEngine;

namespace Game.Interaction
{
    public class CookingStation : MonoBehaviour, IInteractable
    {
        [SerializeField] private CookingMinigame cookingMinigame;
        [SerializeField] private TextMeshProUGUI interactPrompt;
        [SerializeField] private AudioClip cookingSound;

        private HotbarUI hotbar;
        private PlayerInventory inventory;
        private AirQualitySystem airQuality;
        private bool isNearby = false;

        private void Start()
        {
            hotbar = FindFirstObjectByType<HotbarUI>();
            inventory = FindFirstObjectByType<PlayerInventory>();
            airQuality = FindFirstObjectByType<AirQualitySystem>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                isNearby = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                isNearby = false;
        }

        public void Interact(GameObject interactor)
        {
            if (!isNearby || cookingMinigame.IsActive) return;
            
            string selectedFood = hotbar.GetCurrentSelectedItem();
            int quantity = hotbar.GetCurrentSelectedQuantity();

            // Check if selected item is cookable
            if (!IsCookable(selectedFood) || quantity == 0)
            {
                if (interactPrompt)
                    interactPrompt.text = "Select food to cook!";
                return;
            }

            // Check if player has wood
            if (!inventory.HasItem("wood", 1))
            {
                if (interactPrompt)
                    interactPrompt.text = "Need wood to cook!";
                return;
            }

            // Start cooking
            float energy = hotbar.GetFoodEnergy(selectedFood);
            cookingMinigame.StartCooking(selectedFood, energy, inventory, airQuality);

            if (cookingSound)
                AudioSource.PlayClipAtPoint(cookingSound, transform.position);
        }

        public void StopCooking()
        {
            cookingMinigame.StopCooking();
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