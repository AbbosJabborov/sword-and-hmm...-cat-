using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        public Dictionary<string, int> Items = new();
        
        public UnityEvent<string, int> OnItemAdded = new(); // itemName, newQuantity
        public UnityEvent<string, int> OnItemRemoved = new(); // itemName, newQuantity

        public void AddItem(string itemName, int quantity = 1)
        {
            if (!Items.ContainsKey(itemName))
                Items[itemName] = 0;
            
            Items[itemName] += quantity;
            OnItemAdded?.Invoke(itemName, Items[itemName]);
        }

        public bool RemoveItem(string itemName, int quantity = 1)
        {
            if (!Items.ContainsKey(itemName) || Items[itemName] < quantity)
                return false;

            Items[itemName] -= quantity;
            if (Items[itemName] <= 0)
                Items.Remove(itemName);
            
            OnItemRemoved?.Invoke(itemName, Items.ContainsKey(itemName) ? Items[itemName] : 0);
            return true;
        }

        public int GetItemCount(string itemName)
        {
            return Items.ContainsKey(itemName) ? Items[itemName] : 0;
        }

        public bool HasItem(string itemName, int quantity = 1)
        {
            return GetItemCount(itemName) >= quantity;
        }

        public void ClearInventory()
        {
            Items.Clear();
        }
    }
}