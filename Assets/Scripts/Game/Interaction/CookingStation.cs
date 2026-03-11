using Game.Inventory;
using Game.Systems;
using UnityEngine;

namespace Game.Interaction
{
    public class CookingStation : MonoBehaviour, IInteractable
    {
        [Header("Audio/Effects")]
        [SerializeField] private AudioClip cookingSound;
        [SerializeField] private GameObject cookingEffect;

        [Header("Recipes")]
        [SerializeField] private float mushroomCookingCost = 1f; // pollution percentage
        [SerializeField] private float berriesCookingCost = 0.5f;

        public void Interact(GameObject interactor)
        {
            Debug.Log("Player opened cooking menu at station");
            // TODO: Open cooking UI panel here
            // CookingUI.Instance.Show(interactor.GetComponent<PlayerInventory>(), this);
        }

        public bool TryCookMushroom(PlayerInventory inventory, AirQualitySystem airQuality, HungerSystem hunger)
        {
            if (!inventory.RemoveItem("mushroom", 1))
            {
                Debug.Log("Not enough mushrooms!");
                return false;
            }

            if (!inventory.RemoveItem("wood", 1))
            {
                Debug.Log("Not enough wood!");
                inventory.AddItem("mushroom", 1); // Give back the mushroom
                return false;
            }

            // Cook successful
            inventory.AddItem("cooked_mushroom", 1);
            airQuality.AddPollution(mushroomCookingCost);
            PlayCookingEffects();
            
            Debug.Log("Cooked mushroom!");
            return true;
        }

        public bool TryCookBerries(PlayerInventory inventory, AirQualitySystem airQuality, HungerSystem hunger)
        {
            if (!inventory.RemoveItem("berry", 2))
            {
                Debug.Log("Need 2 berries to cook!");
                return false;
            }

            if (!inventory.RemoveItem("wood", 1))
            {
                Debug.Log("Not enough wood!");
                inventory.AddItem("berry", 2); // Give back the berries
                return false;
            }

            // Cook successful
            inventory.AddItem("cooked_berries", 1);
            airQuality.AddPollution(berriesCookingCost);
            PlayCookingEffects();
            
            Debug.Log("Cooked berries!");
            return true;
        }

        private void PlayCookingEffects()
        {
            if (cookingSound)
                AudioSource.PlayClipAtPoint(cookingSound, transform.position);
            if (cookingEffect)
                Instantiate(cookingEffect, transform.position, Quaternion.identity);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.7f);
        }
    }
}