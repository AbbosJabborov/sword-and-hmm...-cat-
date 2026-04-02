using Game.Interaction;
using Game.Inventory;
using UnityEngine;

namespace Game.Plants
{
    public class PlantedPlant : MonoBehaviour, IInteractable
    {
        [Header("Growth Settings")]
        [SerializeField] private float baseGrowthTime = 60f; // Seconds to mature
        [SerializeField] private Renderer plantRenderer;

        [Header("Visual Stages")]
        [SerializeField] private GameObject stage0Prefab; // Sprout
        [SerializeField] private GameObject stage1Prefab; // Growing
        [SerializeField] private GameObject stage2Prefab; // Tall
        [SerializeField] private GameObject stage3Prefab; // Mature

        private string seedName;
        private ItemData seedData;
        private float growthTime;
        private float elapsedTime;
        private PlantingPoint plantingPoint;
        private ItemDatabase itemDatabase;
        private int currentStage = -1;

        public float GrowthPercent => Mathf.Clamp01(elapsedTime / growthTime);
        public bool IsMatured => GrowthPercent >= 1.0f;

        private void Update()
        {
            if (!IsMatured)
            {
                elapsedTime += Time.deltaTime;
                UpdateVisuals();

                if (IsMatured)
                {
                    OnMatured();
                }
            }
        }

        public void Initialize(string seedName, ItemData seedData, PlantingPoint plantingPoint)
        {
            this.seedName = seedName;
            this.seedData = seedData;
            this.plantingPoint = plantingPoint;
            this.elapsedTime = 0f;
            
            // Use growthTime from ItemData if available, otherwise use default
            this.growthTime = seedData.growthTime > 0 ? seedData.growthTime : baseGrowthTime;

            itemDatabase = FindFirstObjectByType<ItemDatabase>();
            
            UpdateVisuals();

            Debug.Log($"[PLANT] Initialized {seedName}. Growth time: {growthTime}s, Growth amount: {seedData.growAmount}");
        }

        private void UpdateVisuals()
        {
            float percent = GrowthPercent;
            int newStage = GetStage(percent);

            if (newStage != currentStage)
            {
                currentStage = newStage;
                UpdateStageVisuals();
            }

            // Scale gradually
            float scale = Mathf.Lerp(0.5f, 1.2f, percent);
            transform.localScale = Vector3.one * scale;

            // Color brightens as it matures
            if (plantRenderer)
            {
                Color matureColor = new Color(0.2f, 1f, 0.2f); // Green
                Color seedColor = new Color(0.6f, 0.6f, 0.6f); // Grey
                Color newColor = Color.Lerp(seedColor, matureColor, percent);
                plantRenderer.material.color = newColor;
            }
        }

        private int GetStage(float growthPercent)
        {
            if (growthPercent < 0.25f) return 0;
            if (growthPercent < 0.50f) return 1;
            if (growthPercent < 0.75f) return 2;
            return 3;
        }

        private void UpdateStageVisuals()
        {
            // Disable all stages
            if (stage0Prefab) stage0Prefab.SetActive(false);
            if (stage1Prefab) stage1Prefab.SetActive(false);
            if (stage2Prefab) stage2Prefab.SetActive(false);
            if (stage3Prefab) stage3Prefab.SetActive(false);

            // Enable current stage
            switch (currentStage)
            {
                case 0:
                    if (stage0Prefab) stage0Prefab.SetActive(true);
                    break;
                case 1:
                    if (stage1Prefab) stage1Prefab.SetActive(true);
                    break;
                case 2:
                    if (stage2Prefab) stage2Prefab.SetActive(true);
                    break;
                case 3:
                    if (stage3Prefab) stage3Prefab.SetActive(true);
                    break;
            }

            Debug.Log($"[PLANT] {seedName} stage: {currentStage} ({GrowthPercent * 100:F0}%)");
        }

        private void OnMatured()
        {
            Debug.Log($"[PLANT] {seedName} is now mature and ready to harvest!");
        }

        public void Interact(GameObject interactor)
        {
            if (!IsMatured)
            {
                Debug.Log($"[PLANT] {seedName} is not ready to harvest yet ({GrowthPercent * 100:F0}%)");
                return;
            }

            Harvest(interactor);
        }

        public void Harvest(GameObject harvester)
        {
            PlayerInventory inventory = harvester.GetComponent<PlayerInventory>();
            if (!inventory)
            {
                Debug.LogError("[PLANT] Could not find inventory!");
                return;
            }

            // Get harvest item name (remove "_seed" suffix)
            string harvestName = seedName.Replace("_seed", "");
            int harvestAmount = seedData.growAmount;

            // Add harvested items to inventory
            inventory.AddItem(harvestName, harvestAmount);

            Debug.Log($"[PLANT] Harvested {harvestAmount}x {harvestName} from {seedName}!");

            // Notify planting point
            if (plantingPoint)
                plantingPoint.OnPlantHarvested();

            // Destroy plant
            Destroy(gameObject);
        }
    }
}