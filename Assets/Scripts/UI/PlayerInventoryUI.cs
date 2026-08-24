using Game.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerInventoryUI : MonoBehaviour
    {
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private Text inventoryText;

        private void OnEnable()
        {
            if (inventory)
            {
                inventory.OnItemAdded.AddListener(UpdateInventoryUI);
                inventory.OnItemRemoved.AddListener(UpdateInventoryUI);
            }
        }

        private void OnDisable()
        {
            if (inventory)
            {
                inventory.OnItemAdded.RemoveListener(UpdateInventoryUI);
                inventory.OnItemRemoved.RemoveListener(UpdateInventoryUI);
            }
        }

        private void UpdateInventoryUI(string itemName, int quantity)
        {
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            if (!inventoryText || !inventory) return;

            string display = "Inventory:\n";
            foreach (var item in inventory.Items)
            {
                display += $"{item.Key}: {item.Value}\n";
            }

            inventoryText.text = display;
        }
    }
}