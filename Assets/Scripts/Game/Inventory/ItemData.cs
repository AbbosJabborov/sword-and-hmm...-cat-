using UnityEngine;

namespace Game.Inventory
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
    public class ItemData : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public int energyAmount;
    }
}