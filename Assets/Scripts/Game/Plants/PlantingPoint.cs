using Game.Interaction;
using Game.Inventory;
using Game.Plants;
using UnityEngine;

namespace Game.Plants
{
    public class PlantingPoint : MonoBehaviour, IInteractable
    {
        [SerializeField] private PlantedPlant plantedPlantPrefab;
        [SerializeField] private float plantingRange = 1.5f;

        private bool _isOccupied = false;

        public void Interact(GameObject interactor)
        {
            if (_isOccupied)
            {
                Debug.Log("Planting spot already occupied!");
                return;
            }

            var inventory = interactor.GetComponent<PlayerInventory>();
            if (inventory == null) return;

            // Player gets to choose what to plant (berry or mushroom)
            // For now, just plant berries
            if (inventory.HasItem("berry", 1))
            {
                inventory.RemoveItem("berry", 1);
                PlantBerry();
            }
            else if (inventory.HasItem("mushroom", 1))
            {
                inventory.RemoveItem("mushroom", 1);
                PlantMushroom();
            }
            else
            {
                Debug.Log("No seeds to plant!");
            }
        }

        private void PlantBerry()
        {
            PlantedPlant newPlant = Instantiate(plantedPlantPrefab, transform.position, Quaternion.identity);
            newPlant.GetComponent<PlantedPlant>().plantType = PlantedPlant.PlantType.Berry;
            _isOccupied = true;
            Debug.Log("Berry planted!");
        }

        private void PlantMushroom()
        {
            PlantedPlant newPlant = Instantiate(plantedPlantPrefab, transform.position, Quaternion.identity);
            newPlant.GetComponent<PlantedPlant>().plantType = PlantedPlant.PlantType.Mushroom;
            _isOccupied = true;
            Debug.Log("Mushroom planted!");
        }

        // Call this if plant dies or is harvested permanently
        public void ResetPoint()
        {
            _isOccupied = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, Vector3.one * plantingRange);
        }
    }
}