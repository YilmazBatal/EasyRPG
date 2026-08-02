using Assets._Project.Scripts.Enums;

namespace TextBasedRPG.Core.Items
{
    public class Weapon : Item
    {
        public int WeaponATK { get; set; }
        public WeaponType WeaponType { get; set; }
        public int RequiredLevel { get; set; }
        public int Upgrade { get; set; }

        public Weapon(string id, ItemType itemType, string name, string? description, int price, Rarity rarity, int quantity, int weaponAtk, WeaponType weaponType, int requiredLevel, int upgrade) : base(id, itemType, name, description, price, rarity, quantity)
        {
            WeaponATK = weaponAtk;
            WeaponType = weaponType;
            RequiredLevel = requiredLevel;
            Upgrade = upgrade;
        }
    }
}