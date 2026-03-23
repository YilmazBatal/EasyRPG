using System;
using System.Collections.Generic;
using TextBasedRPG.Interfaces;

namespace TextBasedRPG.Core.Entities
{
    public abstract class Entity : IDamageable
    {
        public double Scaling = 1.3; 
        public int EliteChance = 5; // %
        public int LevelInterval = 3;

        // JSON datas
        public string ID = string.Empty;
        public string Name = string.Empty;
        public int BaseHP;
        public int BaseATK;
        public int BaseDEF;
        public int BaseSPD;
        public int Level;
        public Dictionary<string, int>? LootTable;  // ID, Chances%
        public List<string> Locations;
        public double GoldMultiplier;
        public EntityType EntityType;

        // Runtime datas
        public double PowerScore => (BaseHP * 0.1) + (BaseATK * 2) + (BaseDEF * 1.5) + (BaseSPD * 0.5);
        public int TotalHP => (int)Math.Round((decimal)BaseHP + (decimal)(BaseHP * GeneratedLevel * 20 / 100 * (isElite ? Scaling : 1)));
        public int GoldReward => (int)Math.Round((decimal)BaseHP + (decimal)(BaseHP * GeneratedLevel * 20 / 100 * (isElite ? Scaling : 1)));
        public int CurHP { get; set; }
        public int TotalATK => (int)Math.Round((decimal)BaseATK + (decimal)(BaseATK * GeneratedLevel * 20/100 * (isElite ? Scaling : 1)));
        public int TotalDEF => (int)Math.Round((decimal) BaseDEF + (decimal) (BaseDEF * GeneratedLevel * 5/100 * (isElite? Scaling : 1)));
        public int CurrentSPD => (int)Math.Round((decimal) BaseSPD + (decimal) (BaseSPD + GeneratedLevel));
        public int GeneratedLevel { get; set; }
        public bool isElite { get; set; }
        public bool IsAlive => CurHP > 0;

        public abstract void Initialize(int playerLevel, int levelCap);

        public void TakeDamage(int amount)
        {
            CurHP -= Math.Max(1, amount);
            if (CurHP < 0) CurHP = 0;
        }
    }
}
