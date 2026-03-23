namespace TextBasedRPG.Core.Items
{
    public class Material : Item
    {
        public int MaxStack { get; set; } = 64;
        public Material(string id, ItemType itemType, string name, string? description, int price, Rarity rarity, int quantity,int maxQuantity) : base(id, itemType, name, description, price, rarity, quantity)
        {
            MaxStack = maxQuantity;
        }
    }
}
