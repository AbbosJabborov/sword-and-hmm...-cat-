using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Inventory
{
    public class ItemSlotUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;

        public void Setup(ItemData item)
        {
            if (item == null) return;

            if (icon != null)
                icon.sprite = item.icon;

            if (nameText != null)
                nameText.text = item.itemName;
        }
    }
}