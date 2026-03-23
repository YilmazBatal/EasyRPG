using System.Collections.Generic;

namespace TextBasedRPG.Models
{
    public class Player
    {
        public string? Class { get; set; }
        public int? Level { get; set; }
        public int? Experience { get; set; }
        public int? Gold { get; set; }
        public int? CurHP { get; set; }
        public string? ActiveLocation { get; set; }
        public int? UnlockedUntill { get; set; }
        public string? EquippedWeapon { get; set; }
        public string? EquippedArmor { get; set; }
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
        public string? ID { get; set; }
        public int Quantity { get; set; }
    }
}
