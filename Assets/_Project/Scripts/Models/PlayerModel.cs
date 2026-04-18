using System.Collections.Generic;

namespace TextBasedRPG.Models
{
    public class Player
    {
        public string? Class { get; set; }
        public int? Level { get; set; }
        public int? Experience { get; set; }
        public int? TotalExp { get; set; }
        public int? Gold { get; set; }
        public int? CurHP { get; set; }
        public string? ActiveLocation { get; set; }
        public int? UnlockedUntill { get; set; }
        public int? Deaths { get; set; }
        public int? EntitiesSlayed { get; set; }
        public int? HeaviestDamage { get; set; }
        public EquippedWeaponData? EquippedWeapon { get; set; }
        public EquippedArmorData? EquippedArmor { get; set; }
        public List<InventoryData>? Inventory { get; set; }
        public StatData? Stats { get; set; }
    }
    public class StatData
    {
        public int? UnusedStatPoints { get; set; }
        public int? InvestedSTR { get; set; }
        public int? InvestedVIT { get; set; }
        public int? InvestedDEX { get; set; }
        public int? InvestedAGI { get; set; }
    }
    public class InventoryData
    {
        public string InstanceID { get; set; }
        public string? ID { get; set; }
        public int Quantity { get; set; }
        public int Upgrade { get; set; }

        public bool ShouldSerializeQuantity()
        {
            if (string.IsNullOrEmpty(ID)) return true;
            return !(ID.StartsWith("W") || ID.StartsWith("A"));
        }

        public bool ShouldSerializeUpgrade()
        {
            if (string.IsNullOrEmpty(ID)) return false;
            return ID.StartsWith("W") || ID.StartsWith("A");
        }
    }
    public class EquippedWeaponData
    {
        public string? ID { get; set; }
        public int Upgrade { get; set; }
    }
     public class EquippedArmorData
    {
        public string? ID { get; set; }
        public int Upgrade { get; set; }
    }
}
