using System.Collections.Generic;
using UnityEngine;

namespace Game.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        private readonly List<ItemData> _items = new();

        public event System.Action<ItemData> OnItemAdded;
        public IReadOnlyList<ItemData> Items => _items;

        public void AddItem(ItemData item)
        {
            if (item == null) return;

            _items.Add(item);
            Debug.Log($"[Inventory] Added: {item.itemName}");
            OnItemAdded?.Invoke(item);
        }

        public bool HasItem(ItemData item) => _items.Contains(item);

        public void RemoveItem(ItemData item)
        {
            if (_items.Remove(item))
                Debug.Log($"[Inventory] Removed: {item.itemName}");
        }
    }
}