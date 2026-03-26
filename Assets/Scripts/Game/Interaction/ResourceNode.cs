using Game.Inventory;
using UnityEngine;

namespace Game.Interaction
{
    public class ResourceNode : MonoBehaviour, IInteractable
    {
        [SerializeField] private ItemData itemData; // The item this node produces when harvested
        [SerializeField] private ItemData seedData; // Optional: seed produced when extracting (for later)
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

        private void Start()
        {
            if (!itemData)
                Debug.LogError($"[RESOURCE] {gameObject.name} has no ItemData assigned!");
        }

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
                Debug.Log($"[RESOURCE] {itemData.itemName} is depleted. Will respawn in {respawnTimer:F0}s");
                return;
            }

            if (!itemData)
            {
                Debug.LogError("[RESOURCE] ItemData is null!");
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

            inventory.AddItem(itemData.itemName, amount);
            PlayEffects(harvestSound, harvestEffect);
            Deplete();
            
            Debug.Log($"[RESOURCE] Harvested {amount}x {itemData.itemName}");
        }

        private void Deplete()
        {
            canHarvest = false;
            respawnTimer = respawnTime;
            if (visualModel)
                visualModel.SetActive(false);
            
            Debug.Log($"[RESOURCE] {itemData.itemName} depleted. Will respawn in {respawnTime}s");
        }

        private void Respawn()
        {
            canHarvest = true;
            if (visualModel)
                visualModel.SetActive(true);
            Debug.Log($"[RESOURCE] {itemData.itemName} has respawned!");
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