using System.Collections.Generic;
using Game.Inventory;
using UnityEngine;

namespace Game
{
    public class ItemDiscoveryTracker : MonoBehaviour
    {
        [SerializeField] private ItemDatabase itemDatabase;
        private Dictionary<string, ItemDiscoveryData> discoveries = new Dictionary<string, ItemDiscoveryData>();
    
        public class ItemDiscoveryData
        {
            public string itemName;
            public bool hasDiscovered = false;
            public bool hasEaten = false;
            public bool hasCooked = false;
            public float timeDiscovered;
            public Sprite illustrationSprite; // Null until discovered
        }
    
        public void DiscoverItem(ItemData item)
        {
            if (!discoveries.ContainsKey(item.itemName))
            {
                discoveries[item.itemName] = new ItemDiscoveryData
                {
                    itemName = item.itemName,
                    hasDiscovered = true,
                    timeDiscovered = Time.time,
                    illustrationSprite = null // Silhouette only
                };
            
                Debug.Log("[JOURNAL] Discovered: " + item.itemName);
            }
        }
    
        public void RecordItemEaten(ItemData item)
        {
            if (discoveries.ContainsKey(item.itemName))
            {
                discoveries[item.itemName].hasEaten = true;
                discoveries[item.itemName].illustrationSprite = item.imageIcon; // Reveal illustration
            
                Debug.Log("[JOURNAL] Revealed illustration for: " + item.itemName);
            }
        }
    
        public void RecordItemCooked(ItemData item)
        {
            if (discoveries.ContainsKey(item.itemName))
            {
                discoveries[item.itemName].hasCooked = true;
            
                Debug.Log("[JOURNAL] Cooked: " + item.itemName);
            }
        }
    
        public List<ItemDiscoveryData> GetDiscoveredItems()
        {
            return new List<ItemDiscoveryData>(discoveries.Values);
        }
    
        public bool HasItemBeenDiscovered(string itemName)
        {
            return discoveries.ContainsKey(itemName) && discoveries[itemName].hasDiscovered;
        }
    }
}