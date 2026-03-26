using Game.Inventory;
using Game.Player;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Systems
{
    public class HungerSystem : MonoBehaviour
    {
        [SerializeField] private float maxHunger = 100f;
        [SerializeField] private float hungerDecreaseRate = 0.5f;
        [SerializeField] private float minHungerForMovement = 10f;
        [SerializeField] private float eatingDuration = 0.5f;
        [SerializeField] private ItemDatabase itemDatabase;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI eatingProgressText;

        public float CurrentHunger { get; private set; }
        public float HungerPercent => CurrentHunger / maxHunger;
        public bool IsExhausted => CurrentHunger <= minHungerForMovement;
        
        [FormerlySerializedAs("OnHungerChanged")] public UnityEvent<float> onHungerChanged = new();
        [FormerlySerializedAs("OnExhausted")] public UnityEvent onExhausted = new();
        [FormerlySerializedAs("OnFed")] public UnityEvent onFed = new();

        private PlayerMovement movement;
        private PlayerInventory inventory;
        private Game.Interaction.Interact interact;
        private Coroutine eatingCoroutine;

        private void Awake()
        {
            movement = GetComponent<PlayerMovement>();
            inventory = FindFirstObjectByType<PlayerInventory>();
            interact = FindFirstObjectByType<Game.Interaction.Interact>();
        }

        private void Start()
        {
            if (!itemDatabase)
                itemDatabase = FindFirstObjectByType<ItemDatabase>();

            if (!itemDatabase)
                Debug.LogError("[HUNGER] ItemDatabase not found!");

            CurrentHunger = maxHunger * 0.6f;
            onHungerChanged?.Invoke(HungerPercent);
            Debug.Log($"[HUNGER] System initialized. Current hunger: {CurrentHunger}/{maxHunger} ({HungerPercent * 100:F1}%)");
        }

        private void Update()
        {
            DecreaseHunger(hungerDecreaseRate * Time.deltaTime);
        }

        public void StartEating(string foodName)
        {
            float energy = GetFoodEnergy(foodName);
            
            if (energy <= 0)
            {
                Debug.LogWarning($"[HUNGER] Food '{foodName}' has no energy value!");
                return;
            }

            Debug.Log($"[HUNGER] Starting eating animation for: {foodName} (+{energy} hunger)");

            if (eatingCoroutine != null)
                StopCoroutine(eatingCoroutine);

            eatingCoroutine = StartCoroutine(EatingAnimation(foodName, energy));
        }

        private IEnumerator EatingAnimation(string foodName, float energy)
        {
            float elapsed = 0f;

            while (elapsed < eatingDuration)
            {
                elapsed += Time.deltaTime;
                float fillAmount = Mathf.Clamp01(elapsed / eatingDuration);

                if (interact)
                    interact.UpdateProgressCircle(fillAmount);

                if (eatingProgressText)
                    eatingProgressText.text = $"Eating... {(fillAmount * 100):F0}%";

                Debug.Log($"[HUNGER] Eating progress: {(fillAmount * 100):F1}%");

                yield return null;
            }

            CompleteEating(foodName, energy);
        }

        private void CompleteEating(string foodName, float energy)
        {
            Debug.Log($"[HUNGER] Finished eating {foodName}");

            Eat(energy);

            if (inventory && inventory.RemoveItem(foodName, 1))
            {
                Debug.Log($"[HUNGER] Removed {foodName} from inventory. Remaining: {inventory.GetItemCount(foodName)}");
            }
            else
            {
                Debug.LogWarning($"[HUNGER] Failed to remove {foodName} from inventory!");
            }

            if (interact)
                interact.UpdateProgressCircle(0f);

            if (eatingProgressText)
                eatingProgressText.text = "";

            eatingCoroutine = null;
        }

        public void Eat(float amount)
        {
            float oldHunger = CurrentHunger;
            CurrentHunger = Mathf.Min(CurrentHunger + amount, maxHunger);
            onHungerChanged?.Invoke(HungerPercent);
            onFed?.Invoke();

            Debug.Log($"[HUNGER] Hunger increased by {amount}. {oldHunger:F1} → {CurrentHunger:F1} ({HungerPercent * 100:F1}%)");
        }

        private void DecreaseHunger(float amount)
        {
            CurrentHunger = Mathf.Max(CurrentHunger - amount, 0);
            onHungerChanged?.Invoke(HungerPercent);

            if (CurrentHunger == 0)
            {
                Debug.Log($"[HUNGER] Player is exhausted!");
                onExhausted?.Invoke();
            }
        }

        public void SetHunger(float amount)
        {
            CurrentHunger = Mathf.Clamp(amount, 0, maxHunger);
            onHungerChanged?.Invoke(HungerPercent);
            Debug.Log($"[HUNGER] Hunger set to {CurrentHunger}/{maxHunger} ({HungerPercent * 100:F1}%)");
        }

        public float GetFoodEnergy(string foodName)
        {
            if (!itemDatabase)
            {
                Debug.LogError("[HUNGER] ItemDatabase not assigned!");
                return 0f;
            }

            ItemData itemData = itemDatabase.GetItemByName(foodName);
            if (!itemData)
            {
                Debug.LogWarning($"[HUNGER] Food '{foodName}' not found in database!");
                return 0f;
            }

            Debug.Log($"[HUNGER] Food energy lookup: {foodName} = {itemData.energyAmount}");
            return itemData.energyAmount;
        }
    }
}