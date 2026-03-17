using DG.Tweening;
using Game.Inventory;
using Game.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Cooking
{
    public class CookingMinigame : MonoBehaviour
    {
        [Header("Cooking Bar")]
        [SerializeField] private Image cookingBarFill;
        [SerializeField] private Image cookingBarBackground;
        [SerializeField] private TextMeshProUGUI cookingText;
        [SerializeField] private Canvas cookingCanvas;

        [Header("Zones")]
        [SerializeField] private float perfectStart = 0.6f; // 60% fill = perfect start
        [SerializeField] private float perfectEnd = 0.8f; // 80% fill = perfect end
        [SerializeField] private float goodStart = 0.4f; // 40% fill = good start
        [SerializeField] private float goodEnd = 0.95f; // 95% fill = good end
        [SerializeField] private float cookSpeed = 0.5f; // How fast the bar fills (0-1 per second)

        [Header("Energy Multipliers")]
        [SerializeField] private float perfectMultiplier = 3f;
        [SerializeField] private float goodMultiplier = 1.5f;
        [SerializeField] private float burntMultiplier = 0f;

        private float fillAmount = 0f;
        private bool isCoking = false;
        private string foodName = "";
        private float baseEnergy = 0f;
        private PlayerInventory inventory;
        private AirQualitySystem airQuality;

        private void Start()
        {
            if (cookingCanvas)
                cookingCanvas.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!isCoking) return;

            // Fill bar while cooking
            fillAmount += cookSpeed * Time.deltaTime;
            fillAmount = Mathf.Clamp01(fillAmount);

            if (cookingBarFill)
                cookingBarFill.fillAmount = fillAmount;

            UpdateVisuals();
        }

        public void StartCooking(string foodName, float energy, PlayerInventory inventory, AirQualitySystem airQuality)
        {
            this.foodName = foodName;
            baseEnergy = energy;
            this.inventory = inventory;
            this.airQuality = airQuality;
            fillAmount = 0f;
            isCoking = true;

            if (cookingCanvas)
                cookingCanvas.gameObject.SetActive(true);
        }

        public void StopCooking()
        {
            if (!isCoking) return;

            isCoking = false;
            EvaluateCooking();
        }

        private void EvaluateCooking()
        {
            string result = "";
            float multiplier = 0f;
            Color zoneColor = Color.white;

            if (fillAmount >= perfectStart && fillAmount <= perfectEnd)
            {
                result = "PERFECT! ★★★";
                multiplier = perfectMultiplier;
                zoneColor = Color.yellow;
            }
            else if (fillAmount >= goodStart && fillAmount <= goodEnd)
            {
                result = "Good ✓";
                multiplier = goodMultiplier;
                zoneColor = Color.green;
            }
            else
            {
                result = "Burnt ✗";
                multiplier = burntMultiplier;
                zoneColor = Color.red;
            }

            float finalEnergy = baseEnergy * multiplier;

            if (cookingText)
            {
                cookingText.text = $"{result}\n{foodName}\n+{finalEnergy:F0} energy";
                cookingText.color = zoneColor;
            }

            // Add pollution
            if (airQuality)
                airQuality.AddPollution(1f);

            // Create cooked item name based on result
            string cookedName = $"{foodName}_cooked";
            if (multiplier == perfectMultiplier)
                cookedName = $"{foodName}_perfect";
            else if (multiplier == goodMultiplier)
                cookedName = $"{foodName}_good";
            else if (multiplier == 0f)
                cookedName = $"{foodName}_burnt";

            // Add to inventory with energy metadata (simple version)
            if (inventory)
            {
                inventory.RemoveItem(foodName, 1);
                inventory.RemoveItem("wood", 1);
                inventory.AddItem(cookedName, 1);
            }

            // Close after 2 seconds
            DOVirtual.DelayedCall(2f, () => {
                if (cookingCanvas)
                    cookingCanvas.gameObject.SetActive(false);
            });
        }

        private void UpdateVisuals()
        {
            // Change bar color based on zone
            if (cookingBarFill)
            {
                if (fillAmount >= perfectStart && fillAmount <= perfectEnd)
                    cookingBarFill.color = Color.yellow; // Perfect zone
                else if (fillAmount >= goodStart && fillAmount <= goodEnd)
                    cookingBarFill.color = Color.green; // Good zone
                else
                    cookingBarFill.color = Color.red; // Burnt zone
            }
        }

        public bool IsActive => isCoking;
    }
}