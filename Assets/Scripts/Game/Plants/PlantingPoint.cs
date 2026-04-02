using Game.Interaction;
using Game.Inventory;
using UnityEngine;

namespace Game.Plants
{
    public class PlantingPoint : MonoBehaviour, IInteractable
    {
        [SerializeField] private float interactRange = 2f;
        [SerializeField] private GameObject plantPrefab;
        [SerializeField] private Transform plantSpawnPoint; // Where the plant grows

        private PlantedPlant currentPlant;
        private ItemDatabase itemDatabase;

        public bool IsOccupied => currentPlant != null;

        private void Start()
        {
            itemDatabase = FindFirstObjectByType<ItemDatabase>();
            if (!itemDatabase)
                Debug.LogError("[PLANT] ItemDatabase not found!");
            if (!plantPrefab)
                Debug.LogError("[PLANT] Plant prefab not assigned!");
            
            Debug.Log("[PLANT] PlantingPoint initialized at " + transform.position);
        }

        public void Interact(GameObject interactor)
        {
            // If already has a plant, that plant handles interaction
            if (IsOccupied)
            {
                Debug.Log("[PLANT] This planting point is occupied, cannot plant here");
                return;
            }

            // Get interactor's inventory and hotbar
            PlayerInventory inventory = interactor.GetComponent<PlayerInventory>();
            UI.HotbarUI hotbar = FindFirstObjectByType<UI.HotbarUI>();

            if (!inventory || !hotbar)
            {
                Debug.LogError("[PLANT] Could not find inventory or hotbar!");
                return;
            }

            // Get selected item from hotbar
            string selectedItem = hotbar.GetCurrentSelectedItem();
            int quantity = hotbar.GetCurrentSelectedQuantity();

            if (string.IsNullOrEmpty(selectedItem) || quantity == 0)
            {
                Debug.Log("[PLANT] No item selected!");
                return;
            }

            // Check if it's a seed
            ItemData itemData = itemDatabase.GetItemByName(selectedItem);
            if (!itemData || itemData.resourceType != ItemData.ResourceType.Seed)
            {
                Debug.Log($"[PLANT] {selectedItem} is not a seed!");
                return;
            }

            // Plant the seed
            PlantSeed(selectedItem, itemData, inventory);
        }

        public void PlantSeed(string seedName, ItemData seedData, PlayerInventory inventory)
        {
            // Remove seed from inventory
            if (!inventory.RemoveItem(seedName, 1))
            {
                Debug.LogError("[PLANT] Failed to remove seed from inventory!");
                return;
            }

            // Instantiate plant
            Transform spawnPoint = plantSpawnPoint ? plantSpawnPoint : transform;
            GameObject plantObj = Instantiate(plantPrefab, spawnPoint.position, Quaternion.identity, transform);
            
            currentPlant = plantObj.GetComponent<PlantedPlant>();
            if (!currentPlant)
            {
                Debug.LogError("[PLANT] Plant prefab doesn't have PlantedPlant component!");
                Destroy(plantObj);
                return;
            }

            // Initialize plant
            currentPlant.Initialize(seedName, seedData, this);
            
            Debug.Log($"[PLANT] Planted {seedName} at {transform.position}. Will mature in {seedData.growthTime} seconds.");
        }

        public PlantedPlant GetPlant() => currentPlant;

        public void OnPlantHarvested()
        {
            currentPlant = null;
            Debug.Log("[PLANT] Plant harvested, planting point is now empty");
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 1.5f);
        }
    }
}