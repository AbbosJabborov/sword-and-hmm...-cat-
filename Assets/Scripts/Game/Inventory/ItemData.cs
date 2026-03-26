using UnityEngine;

namespace Game.Inventory
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
    public class ItemData : ScriptableObject
    {
        public enum ResourceType { Food, Seed, Water }
        public string itemName;
        public Sprite icon;
        public int energyAmount; // if its food, this is the value that will provide to hunger bar
        public int growAmount; // if we plant one seed, this is the amount of how much food/plant it will grow out as, like 3x
        public ResourceType resourceType;
    }
}