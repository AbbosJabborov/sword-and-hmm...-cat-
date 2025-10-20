using Game.Inventory;
using UnityEngine;

namespace Game.Interaction
{
    public class Pickup : MonoBehaviour, IInteractable
    {
        [Header("Item Settings")]
        [SerializeField] private ItemData itemData;

        [Header("Effects")]
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private GameObject pickupEffect;

        public void Interact(GameObject interactor)
        {
            if (itemData == null)
            {
                Debug.LogWarning("Pickup: No ItemData assigned!");
                return;
            }
            
            var inventory = interactor.GetComponent<PlayerInventory>();
            if (inventory == null)
            {
                Debug.LogWarning("Pickup: Interactor has no PlayerInventory!");
                return;
            }
            
            inventory.AddItem(itemData);
            
            if (pickupSound)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            if (pickupEffect)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            
            Destroy(gameObject);
        }
    }
}