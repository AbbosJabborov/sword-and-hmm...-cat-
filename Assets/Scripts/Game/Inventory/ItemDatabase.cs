using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    [CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<ItemData> items = new();

        public ItemData GetItemByName(string itemName)
        {
            foreach (var item in items)
            {
                if (item.itemName == itemName)
                {
                    Debug.Log($"[DATABASE] Found item: {itemName}");
                    return item;
                }
            }

            Debug.LogWarning($"[DATABASE] Item '{itemName}' not found in database!");
            return null;
        }

        public bool TryGetItem(string itemName, out ItemData itemData)
        {
            itemData = GetItemByName(itemName);
            return itemData != null;
        }

#if UNITY_EDITOR
        public List<ItemData> GetAllItems() => items;
#endif
    }
}