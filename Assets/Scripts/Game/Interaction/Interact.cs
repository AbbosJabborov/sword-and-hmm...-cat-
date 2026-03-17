using Game.Inventory;
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
            currentInteractionType = InteractionType.None;

            foreach (var hit in hits)
            {
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
            if (currentInteractable == null && hotbar)
            {
                string selectedFood = hotbar.GetCurrentSelectedItem();
                if (!string.IsNullOrEmpty(selectedFood))
                {
                    currentInteractionType = InteractionType.Eat;
                    
                    if (interactPrompt)
                        interactPrompt.text = "";
                    
                    if (consumePrompt)
                        consumePrompt.text = "[R] Consume";
                        // consumePrompt.text = $"[R] Eat ({selectedFood})";
                }
                else
                {
                    if (consumePrompt)
                        consumePrompt.text = "";
                }
            }

            // Clear prompts if nothing nearby
            if (currentInteractable == null && currentInteractionType == InteractionType.None)
            {
                if (interactPrompt)
                    interactPrompt.text = "";
                if (consumePrompt)
                    consumePrompt.text = "";
            }
        }

        public void OnInteract(GameObject interactor)
        {
            if (currentInteractable != null && currentInteractionType == InteractionType.Gather)
            {
                Debug.Log($"[INTERACT] Interaction type: Gather");
                currentInteractable.Interact(interactor);
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
                    
                    // Show progress circle
                    if (progressCircleCanvasGroup)
                        progressCircleCanvasGroup.alpha = 1f;
                    if (progressCircleImage)
                        progressCircleImage.fillAmount = 0f;

                    // Start eating
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

                // Hide progress circle
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