using UnityEngine;

namespace Game.Inventory
{
    public class PlayerInventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInventory playerInventory;
        [SerializeField] private Transform gridParent;
        [SerializeField] private GameObject itemSlotPrefab;

        private void Start()
        {
            RefreshUI();
        }

        public void RefreshUI()
        {
            // Clear old slots
            foreach (Transform child in gridParent)
                Destroy(child.gameObject);

            // Recreate slots
            foreach (var item in playerInventory.Items)
            {
                GameObject slot = Instantiate(itemSlotPrefab, gridParent);
                var slotUI = slot.GetComponent<ItemSlotUI>();
                slotUI.Setup(item);
            }
        }
        private void OnEnable()
        {
            playerInventory.OnItemAdded += HandleItemAdded;
        }

        private void OnDisable()
        {
            playerInventory.OnItemAdded -= HandleItemAdded;
        }

        private void HandleItemAdded(ItemData item)
        {
            GameObject slot = Instantiate(itemSlotPrefab, gridParent);
            slot.GetComponent<ItemSlotUI>().Setup(item);
        }

    }
}