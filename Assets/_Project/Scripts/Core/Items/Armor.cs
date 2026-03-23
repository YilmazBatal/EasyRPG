namespace TextBasedRPG.Core.Items
{
    public class Armor : Item
    {
        public int ArmorDef { get; set; }
        public int ExtraHP { get; set; } = 0;
        public int RequiredLevel { get; set; }

        public Armor(string id, ItemType itemType, string name, string? description, int price, Rarity rarity, int quantity, int armorDef, int extraHP, int requiredLevel) : base(id, itemType, name, description, price, rarity, quantity)
        {
            ArmorDef = armorDef;
            ExtraHP = extraHP;
            RequiredLevel = requiredLevel;
        }
    }
}
