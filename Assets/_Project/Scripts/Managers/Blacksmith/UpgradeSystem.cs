using UnityEngine;
using TextBasedRPG.Core.Items;
using Assets._Project.Scripts.Enums;

namespace Assets._Project.Scripts.Managers.Blacksmith
{
    public static class UpgradeSystem
    {
        public static int GetMaxUpgradeLevel(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common: return 5;
                case Rarity.Uncommon: return 5;
                case Rarity.Rare: return 10;
                case Rarity.Epic: return 15;
                case Rarity.Legendary: return 20;
                default: return 5;
            }
        }

        public static float GetRarityMultiplier(Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common: return 1.0f;
                case Rarity.Uncommon: return 1.25f;
                case Rarity.Rare: return 1.5f;
                case Rarity.Epic: return 2.0f;
                case Rarity.Legendary: return 3.0f;
                default: return 1.0f;
            }
        }

        public static int CalculateUpgradedStat(int baseStat, int upgradeLevel)
        {
            if (upgradeLevel == 0) return baseStat;
            return baseStat + Mathf.CeilToInt(baseStat * 0.05f * upgradeLevel);
        }

        public static int CalculateSuccessChance(int currentLevel)
        {
            int chance = 100 - (currentLevel * 4);
            return Mathf.Max(15, chance);
        }

        public static int CalculateGoldCost(int baseValue, int currentLevel, Rarity rarity)
        {
            float rarityMultiplier = GetRarityMultiplier(rarity);
            return Mathf.CeilToInt(baseValue * 0.15f * Mathf.Pow(1 + currentLevel, 1.4f) * rarityMultiplier);
        }

        public static bool RollUpgradeSuccess(int currentLevel)
        {
            int chance = CalculateSuccessChance(currentLevel);
            int roll = Random.Range(1, 101);
            return roll <= chance;
        }
    }
}
