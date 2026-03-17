using Game.Inventory;
using Game.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HotbarUI : MonoBehaviour
    {
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private HotbarSlot[] slots = new HotbarSlot[9];
        [SerializeField] private HungerSystem hungerSystem;

        private int currentSelectedSlot = 0;
        private string[] slotContents = new string[9];

        [System.Serializable]
        public class HotbarSlot
        {
            public Image slotImage;
            public TextMeshProUGUI quantityText;
            public Image highlightImage; 
            public Color highlightedColor = Color.yellow;
            public Color normalColor = Color.white;
        }

        private void OnEnable()
        {
            if (inventory)
            {
                inventory.OnItemAdded.AddListener(UpdateHotbar);
                inventory.OnItemRemoved.AddListener(UpdateHotbar);
            }
        }

        private void OnDisable()
        {
            if(!inventory) return;
            inventory.OnItemAdded.RemoveListener(UpdateHotbar);
            inventory.OnItemRemoved.RemoveListener(UpdateHotbar);
        }

        private void Start()
        {
            if (!hungerSystem)
                hungerSystem = FindFirstObjectByType<HungerSystem>();
            
            UpdateHotbar("", 0);
        }

        // Called from PlayerInputHandler
        public void OnCycleLeft() => CycleSlot(-1);
        public void OnCycleRight() => CycleSlot(1);

        private void CycleSlot(int direction)
        {
            currentSelectedSlot = (currentSelectedSlot + direction + slots.Length) % slots.Length;
            Debug.Log($"[HOTBAR] Cycled to slot {currentSelectedSlot}: {GetCurrentSelectedItem()}");
            UpdateSlotDisplay();
        }

        private void UpdateHotbar(string itemName, int quantity)
        {
            RefreshSlotContents();
            UpdateSlotDisplay();
        }

        private void RefreshSlotContents()
        {
            slotContents = new string[9];
            int slotIndex = 0;

            foreach (var item in inventory.Items)
            {
                if (slotIndex < 9)
                {
                    slotContents[slotIndex] = item.Key;
                    slotIndex++;
                }
            }
        }

        private void UpdateSlotDisplay()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == null) continue;

                string itemInSlot = slotContents[i];
                int quantity = string.IsNullOrEmpty(itemInSlot) ? 0 : inventory.GetItemCount(itemInSlot);

                // Update slot visuals
                if (slots[i].quantityText)
                    slots[i].quantityText.text = quantity > 0 ? quantity.ToString() : "";

                if (slots[i].slotImage)
                    slots[i].slotImage.color = quantity > 0 ? Color.white : Color.grey;

                // Highlight selected slot
                if (slots[i].highlightImage)
                {
                    if (i == currentSelectedSlot)
                        slots[i].highlightImage.color = slots[i].highlightedColor; // Yellow
                    else
                        slots[i].highlightImage.color = slots[i].normalColor; // White
                }
            }
        }

        public string GetCurrentSelectedItem() => slotContents[currentSelectedSlot];
        
        public int GetCurrentSelectedQuantity()
        {
            string item = GetCurrentSelectedItem();
            return string.IsNullOrEmpty(item) ? 0 : inventory.GetItemCount(item);
        }

        public float GetFoodEnergy(string foodName)
        {
            return foodName.ToLower() switch
            {
                "berry" => 5f,
                "mushroom" => 8f,
                _ => 0f
            };
        }
    }
}