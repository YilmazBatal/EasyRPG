namespace TextBasedRPG.Core.Items
{
    public class Consumable : Item
    {
        public string Effect { get; set; }
        public int Value { get; set; }
        public bool CombatItem { get; set; }
        
        public Consumable(string id, ItemType itemType, string name, string? description, int price, Rarity rarity, int quantity, string effect, int value, bool combatItem) : base(id, itemType, name, description, price, rarity, quantity)
        {
            Effect = effect;
            Value = value;
            CombatItem = combatItem;
        }
    }
}
