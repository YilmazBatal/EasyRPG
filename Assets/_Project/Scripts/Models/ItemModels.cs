namespace TextBasedRPG.Models
{
    public class WeaponData
    {
        public  string ID { get; set; }
        public string? ItemType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public string? Rarity { get; set; }
        public int Quantity { get; set; }
        public int WeaponATK { get; set; }
        public string? WeaponType { get; set; }
        public int Level { get; set; }
        public int RequiredLevel { get; set; }
        public int Upgrade { get; set; }
    }
    public class ArmorData
    {
        public  string ID { get; set; }
        public string? ItemType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public string? Rarity { get; set; }
        public int Quantity { get; set; }
        public int ArmorDef { get; set; }
        public int ExtraHP { get; set; }
        public int Level { get; set; }
        public int RequiredLevel { get; set; }
    }
    public class MaterialData
    {
        public  string ID { get; set; }
        public string? ItemType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public string? Rarity { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
    }
    public class ConsumableData
    {
        public  string ID { get; set; }
        public string? ItemType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public string? Rarity { get; set; }
        public int Quantity { get; set; }
        public string? Effect { get; set; }
        public int Value { get; set; }
        public bool CombatItem { get; set; }
    }
}
