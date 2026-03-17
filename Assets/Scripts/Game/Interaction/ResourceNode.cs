using Game.Inventory;
using UnityEngine;

namespace Game.Interaction
{
    public class ResourceNode : MonoBehaviour, IInteractable
    {
        public enum ResourceType { Berry, Mushroom, Tree, Water }
        
        [SerializeField] private ResourceType resourceType;
        [SerializeField] private int harvestAmountMin = 2;
        [SerializeField] private int harvestAmountMax = 4;
        [SerializeField] private float respawnTime = 120f;
        
        [Header("Effects")]
        [SerializeField] private AudioClip harvestSound;
        [SerializeField] private GameObject harvestEffect;
        
        [Header("Visuals")]
        [SerializeField] private GameObject visualModel;

        private float respawnTimer = 0f;
        private bool canHarvest = true;

        private void Update()
        {
            if (!canHarvest)
            {
                respawnTimer -= Time.deltaTime;
                if (respawnTimer <= 0)
                    Respawn();
            }
        }

        public void Interact(GameObject interactor)
        {
            if (!canHarvest)
            {
                Debug.Log($"[RESOURCE] {resourceType} is depleted. Will respawn in {respawnTimer:F0}s");
                return;
            }

            var inventory = interactor.GetComponent<PlayerInventory>();
            if (inventory == null)
            {
                Debug.LogWarning("[RESOURCE] Interactor has no PlayerInventory!");
                return;
            }

            HarvestResource(inventory);
        }

        private void HarvestResource(PlayerInventory inventory)
        {
            if (!canHarvest) return;

            int amount = Random.Range(harvestAmountMin, harvestAmountMax + 1);

            string itemName = resourceType switch
            {
                ResourceType.Berry => "berry",
                ResourceType.Mushroom => "mushroom",
                ResourceType.Tree => "wood",
                ResourceType.Water => "water",
                _ => "unknown"
            };

            inventory.AddItem(itemName, amount);
            PlayEffects(harvestSound, harvestEffect);
            Deplete();
            
            Debug.Log($"[RESOURCE] Harvested {amount}x {itemName} from {resourceType}");
        }

        private void Deplete()
        {
            canHarvest = false;
            respawnTimer = respawnTime;
            if (visualModel)
                visualModel.SetActive(false);
            
            Debug.Log($"[RESOURCE] {resourceType} depleted. Will respawn in {respawnTime}s");
        }

        private void Respawn()
        {
            canHarvest = true;
            if (visualModel)
                visualModel.SetActive(true);
            Debug.Log($"[RESOURCE] {resourceType} has respawned!");
        }

        private void PlayEffects(AudioClip sound, GameObject effect)
        {
            if (sound)
                AudioSource.PlayClipAtPoint(sound, transform.position);
            if (effect)
                Instantiate(effect, transform.position, Quaternion.identity);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }
    }
}