using Game.Inventory;
using Game.Plants;
using Game.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Interaction
{
    public enum InteractionType
    {
        None,
        Gather,
        Cook,
        Plant,
        Harvest,
        Eat
    }

    public class Interact : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactRange = 2f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private TextMeshProUGUI interactPrompt;
        [SerializeField] private TextMeshProUGUI consumePrompt;

        [Header("Progress Circle")]
        [SerializeField] private Image progressCircleImage;
        [SerializeField] private CanvasGroup progressCircleCanvasGroup;

        private IInteractable currentInteractable;
        private HungerSystem hungerSystem;
        private UI.HotbarUI hotbar;
        private CookingStation currentCookingStation;
        private PlantingPoint currentPlantingPoint;
        private PlantedPlant currentPlantedPlant;
        private InteractionType currentInteractionType = InteractionType.None;
        private bool isHoldingConsume = false;

        private void Start()
        {
            hungerSystem = FindFirstObjectByType<HungerSystem>();
            hotbar = FindFirstObjectByType<UI.HotbarUI>();

            if (progressCircleCanvasGroup)
                progressCircleCanvasGroup.alpha = 0f;
        }

        private void Update()
        {
            // Check for nearby interactables
            Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);
            currentInteractable = null;
            currentCookingStation = null;
            currentPlantingPoint = null;
            currentPlantedPlant = null;
            currentInteractionType = InteractionType.None;

            foreach (var hit in hits)
            {
                // Check for PlantedPlant (mature plant to harvest)
                PlantedPlant plantedPlant = hit.GetComponent<PlantedPlant>();
                if (plantedPlant != null && plantedPlant.IsMatured)
                {
                    currentPlantedPlant = plantedPlant;
                    currentInteractionType = InteractionType.Harvest;
                    
                    if (interactPrompt)
                        interactPrompt.text = "[F] Harvest";
                    
                    if (consumePrompt)
                        consumePrompt.text = "";
                    
                    break;
                }

                // Check for CookingStation
                CookingStation cookingStation = hit.GetComponent<CookingStation>();
                if (cookingStation != null)
                {
                    currentCookingStation = cookingStation;
                    currentInteractionType = InteractionType.Cook;
                    
                    if (interactPrompt)
                        interactPrompt.text = "[F] Cook";
                    
                    if (consumePrompt)
                        consumePrompt.text = "";
                    
                    break;
                }

                // Check for PlantingPoint (for planting)
                PlantingPoint plantingPoint = hit.GetComponent<PlantingPoint>();
                if (plantingPoint != null && !plantingPoint.IsOccupied)
                {
                    currentPlantingPoint = plantingPoint;
                    currentInteractionType = InteractionType.Plant;
                    
                    // Show what seed we have selected (if any)
                    string selectedSeed = hotbar ? hotbar.GetCurrentSelectedItem() : "";
                    if (!string.IsNullOrEmpty(selectedSeed))
                    {
                        if (interactPrompt)
                            interactPrompt.text = $"[F] Plant {selectedSeed}";
                    }
                    else
                    {
                        if (interactPrompt)
                            interactPrompt.text = "[F] Plant";
                    }
                    
                    if (consumePrompt)
                        consumePrompt.text = "";
                    
                    break;
                }

                // Check for regular interactable (gather)
                IInteractable interactable = hit.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    currentInteractable = interactable;
                    currentInteractionType = InteractionType.Gather;
                    
                    if (interactPrompt)
                        interactPrompt.text = "[F] Gather";
                    
                    if (consumePrompt)
                        consumePrompt.text = "";
                    
                    break;
                }
            }

            // Check if player has food selected in hotbar
            if (currentInteractable == null && currentCookingStation == null && 
                currentPlantingPoint == null && currentPlantedPlant == null && hotbar)
            {
                string selectedFood = hotbar.GetCurrentSelectedItem();
                if (!string.IsNullOrEmpty(selectedFood))
                {
                    currentInteractionType = InteractionType.Eat;
                    
                    if (interactPrompt)
                        interactPrompt.text = "";
                    
                    if (consumePrompt)
                        consumePrompt.text = $"[R] Eat ({selectedFood})";
                }
                else
                {
                    if (consumePrompt)
                        consumePrompt.text = "";
                }
            }

            // Clear prompts if nothing nearby
            if (currentInteractable == null && currentCookingStation == null && 
                currentPlantingPoint == null && currentPlantedPlant == null && 
                currentInteractionType == InteractionType.None)
            {
                if (interactPrompt)
                    interactPrompt.text = "";
                if (consumePrompt)
                    consumePrompt.text = "";
            }
        }

        public void OnInteract(GameObject interactor)
        {
            switch (currentInteractionType)
            {
                case InteractionType.Gather:
                    if (currentInteractable != null)
                    {
                        Debug.Log($"[INTERACT] Interaction type: Gather");
                        currentInteractable.Interact(interactor);
                    }
                    break;

                case InteractionType.Cook:
                    if (currentCookingStation != null)
                    {
                        Debug.Log($"[INTERACT] Interaction type: Cook");
                        currentCookingStation.Interact(interactor);
                    }
                    break;

                case InteractionType.Plant:
                    if (currentPlantingPoint != null)
                    {
                        Debug.Log($"[INTERACT] Interaction type: Plant");
                        currentPlantingPoint.Interact(interactor);
                    }
                    break;

                case InteractionType.Harvest:
                    if (currentPlantedPlant != null)
                    {
                        Debug.Log($"[INTERACT] Interaction type: Harvest");
                        currentPlantedPlant.Interact(interactor);
                    }
                    break;
            }
        }

        public void OnConsumePressed(GameObject interactor)
        {
            if (currentInteractionType == InteractionType.Eat && hotbar)
            {
                string selectedFood = hotbar.GetCurrentSelectedItem();
                int quantity = hotbar.GetCurrentSelectedQuantity();

                if (!string.IsNullOrEmpty(selectedFood) && quantity > 0)
                {
                    isHoldingConsume = true;
                    Debug.Log($"[CONSUME] Started eating: {selectedFood}");
                    
                    if (progressCircleCanvasGroup)
                        progressCircleCanvasGroup.alpha = 1f;
                    if (progressCircleImage)
                        progressCircleImage.fillAmount = 0f;

                    hungerSystem.StartEating(selectedFood);
                }
                else
                {
                    Debug.Log($"[CONSUME] No food selected or out of stock");
                }
            }
        }

        public void OnConsumeReleased(GameObject interactor)
        {
            if (isHoldingConsume)
            {
                isHoldingConsume = false;
                Debug.Log($"[CONSUME] Released eat action");

                if (progressCircleCanvasGroup)
                    progressCircleCanvasGroup.alpha = 0f;
            }
        }

        public void UpdateProgressCircle(float fillAmount)
        {
            if (progressCircleImage)
                progressCircleImage.fillAmount = fillAmount;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactRange);
        }
    }
}