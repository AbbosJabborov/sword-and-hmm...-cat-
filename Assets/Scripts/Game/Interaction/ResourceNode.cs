using Game.Inventory;
using UnityEngine;

namespace Game.Interaction
{
    public class ResourceNode : MonoBehaviour, IInteractable
    {
        public enum ResourceType { Berry, Mushroom, Tree, Water }
        
        [SerializeField] private ResourceType resourceType;
        [SerializeField] private int harvestAmount = 1;
        [SerializeField] private float respawnTime = 120f; // seconds
        
        [Header("Effects")]
        [SerializeField] private AudioClip harvestSound;
        [SerializeField] private GameObject harvestEffect;
        
        [Header("Visuals")]
        [SerializeField] private GameObject visualModel;

        private float _respawnTimer = 0f;
        private bool _canHarvest = true;

        private void Update()
        {
            if (!_canHarvest)
            {
                _respawnTimer -= Time.deltaTime;
                if (_respawnTimer <= 0)
                    Respawn();
            }
        }

        public void Interact(GameObject interactor)
        {
            if (!_canHarvest)
            {
                Debug.Log($"{resourceType} is depleted. Will respawn in {_respawnTimer:F0}s");
                return;
            }

            var inventory = interactor.GetComponent<PlayerInventory>();
            if (inventory == null)
            {
                Debug.LogWarning("Interactor has no PlayerInventory!");
                return;
            }

            // Add resource to inventory based on type
            string itemName = resourceType switch
            {
                ResourceType.Berry => "berry",
                ResourceType.Mushroom => "mushroom",
                ResourceType.Tree => "wood",
                ResourceType.Water => "water",
                _ => "unknown"
            };

            inventory.AddItem(itemName, harvestAmount);
            PlayEffects();
            Deplete();
        }

        private void Deplete()
        {
            _canHarvest = false;
            _respawnTimer = respawnTime;
            if (visualModel)
                visualModel.SetActive(false);
        }

        private void Respawn()
        {
            _canHarvest = true;
            if (visualModel)
                visualModel.SetActive(true);
        }

        private void PlayEffects()
        {
            if (harvestSound)
                AudioSource.PlayClipAtPoint(harvestSound, transform.position);
            if (harvestEffect)
                Instantiate(harvestEffect, transform.position, Quaternion.identity);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }
    }
}
