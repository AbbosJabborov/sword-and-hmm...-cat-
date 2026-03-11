using Game.Inventory;
using UnityEngine;

namespace Game.Interaction
{
    public class WaterSource : MonoBehaviour, IInteractable
    {
        [SerializeField] private int waterAmount = 5;
        [SerializeField] private AudioClip collectSound;
        [SerializeField] private GameObject collectEffect;

        public void Interact(GameObject interactor)
        {
            var inventory = interactor.GetComponent<PlayerInventory>();
            if (inventory == null)
            {
                Debug.LogWarning("Interactor has no PlayerInventory!");
                return;
            }

            inventory.AddItem("water", waterAmount);
            PlayEffects();
            Debug.Log($"Collected {waterAmount} water");
        }

        private void PlayEffects()
        {
            if (collectSound)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            if (collectEffect)
                Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);
        }
    }
}
