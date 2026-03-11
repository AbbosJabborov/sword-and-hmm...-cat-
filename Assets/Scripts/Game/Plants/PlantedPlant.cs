using Game.Interaction;
using Game.Inventory;
using Game.Systems;
using UnityEngine;

namespace Game.Plants
{
    public class PlantedPlant : MonoBehaviour, IInteractable
    {
        public enum PlantType { Berry, Mushroom }
        
        [SerializeField] public PlantType plantType;
        [SerializeField] private int maxGrowthStage = 5;
        [SerializeField] private float growthTimePerStage = 60f; // seconds
        [SerializeField] private Mesh[] growthModels; // Different mesh for each stage

        [Header("Harvesting")]
        [SerializeField] private int harvestAmount = 2;
        [SerializeField] private bool regrows = true;
        
        [Header("Visuals")]
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private AudioClip harvestSound;
        [SerializeField] private GameObject harvestEffect;

        private int _currentGrowthStage = 0;
        private float _growthTimer = 0f;
        private bool _isWatered = false;
        private AirQualitySystem _airQuality;

        private void Start()
        {
            _airQuality = FindObjectOfType<AirQualitySystem>();
            UpdateVisuals();
        }

        private void Update()
        {
            if (_currentGrowthStage >= maxGrowthStage) return;

            // Grow faster if watered
            float growthRate = _isWatered ? 1.5f : 1f;
            _growthTimer += Time.deltaTime * growthRate;

            if (_growthTimer >= growthTimePerStage)
            {
                _currentGrowthStage++;
                _growthTimer = 0f;
                _isWatered = false; // Need to water again next cycle
                UpdateVisuals();

                // Add air quality when plant grows (purifies air)
                if (_airQuality)
                    _airQuality.AddCleanAir(0.5f);

                Debug.Log($"Plant grew to stage {_currentGrowthStage}");
            }
        }

        public void Water()
        {
            _isWatered = true;
            Debug.Log("Plant watered!");
        }

        public void Interact(GameObject interactor)
        {
            if (_currentGrowthStage < maxGrowthStage)
            {
                Debug.Log($"Plant not ready yet. Growth: {_currentGrowthStage}/{maxGrowthStage}");
                return;
            }

            // Harvest
            var inventory = interactor.GetComponent<PlayerInventory>();
            if (inventory == null)
            {
                Debug.LogWarning("Interactor has no PlayerInventory!");
                return;
            }

            string itemName = plantType == PlantType.Berry ? "berry" : "mushroom";
            inventory.AddItem(itemName, harvestAmount);

            if (harvestSound)
                AudioSource.PlayClipAtPoint(harvestSound, transform.position);
            if (harvestEffect)
                Instantiate(harvestEffect, transform.position, Quaternion.identity);

            if (regrows)
            {
                _currentGrowthStage = 0;
                _growthTimer = 0f;
                _isWatered = false;
                UpdateVisuals();
                Debug.Log("Plant regrew!");
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void UpdateVisuals()
        {
            // Update mesh if we have growth models
            if (growthModels != null && growthModels.Length > _currentGrowthStage && meshFilter)
                meshFilter.mesh = growthModels[_currentGrowthStage];

            // Scale up as it grows
            float scale = 0.5f + _currentGrowthStage * 0.1f;
            transform.localScale = Vector3.one * scale;
        }

        public float GrowthPercent => (float)_currentGrowthStage / maxGrowthStage;
        public bool IsFullyGrown => _currentGrowthStage >= maxGrowthStage;
    }
}