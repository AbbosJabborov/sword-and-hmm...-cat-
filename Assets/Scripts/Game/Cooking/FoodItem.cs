using Game.Interaction;
using Game.Inventory;
using UnityEngine;

namespace Game.Cooking
{
    public class FoodItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private string foodName;
        [SerializeField] private int foodEnergy = 5;
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private GameObject pickupEffect;

        private bool canPickup = true;
        private float spoilTimer = 0f;
        [SerializeField] private float spoilTime = 300f; // Food spoils after 5 minutes if not picked up

        private void Update()
        {
            if (canPickup)
            {
                spoilTimer += Time.deltaTime;
                if (spoilTimer >= spoilTime)
                {
                    Spoil();
                }
            }
        }

        public void SetFood(string name, int energy = 5)
        {
            foodName = name;
            foodEnergy = energy;
        }

        public void Interact(GameObject interactor)
        {
            if (!canPickup) return;

            var inventory = interactor.GetComponent<PlayerInventory>();
            if (inventory == null) return;

            inventory.AddItem(foodName, 1);
            PlayPickupEffects();
            Destroy(gameObject);
        }

        private void Spoil()
        {
            canPickup = false;
            // Optional: Change appearance to show it's spoiled
            GetComponent<Renderer>().material.color = new Color(0.5f, 0.3f, 0.3f);
            
            // Destroy after short delay
            Destroy(gameObject, 5f);
        }

        private void PlayPickupEffects()
        {
            if (pickupSound)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            if (pickupEffect)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}