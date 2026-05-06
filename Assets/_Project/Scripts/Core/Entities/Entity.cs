using System;
using System.Collections.Generic;
using TextBasedRPG.Interfaces;
using TextBasedRPG.Models;

namespace TextBasedRPG.Core.Entities
{
    public abstract class Entity : IDamageable
    {
        public float Scaling = 1.3f; 
        public int EliteChance = 5; // %
        public int LevelInterval = 3;

        // JSON datas
        public string ID = string.Empty;
        public string EntityTypeID = string.Empty;
        public string Name = string.Empty;
        public int BaseHP;
        public int BaseATK;
        public int BaseDEF;
        public int BaseSPD;
        public string EntitySprite;
        public int Level;
        public List<Loots>? Loots;  // ID, Chances%
        public List<string> Locations;
        public float GoldMultiplier;
        public EntityType EntityType;

        // Runtime datas
        public float PowerScore => (BaseHP * 0.1f) + (BaseATK * 2f) + (BaseDEF * 1.5f) + (BaseSPD * 0.5f);
        public int TotalHP => (int)Math.Round(BaseHP + (float)(BaseHP * GeneratedLevel * 20 / 100 * (isElite ? Scaling : 1)));
        public int GoldReward => (int)Math.Round(BaseHP + (float)(BaseHP * GeneratedLevel * 20 / 100 * (isElite ? Scaling : 1)));
        public int CurHP { get; set; }
        public int TotalATK => (int)Math.Round(BaseATK + (float)(BaseATK * GeneratedLevel * 20/100 * (isElite ? Scaling : 1)));
        public int TotalDEF => (int)Math.Round(BaseDEF + (float) (BaseDEF * GeneratedLevel * 5/100 * (isElite? Scaling : 1)));
        public int CurrentSPD => (int)Math.Round(BaseSPD + (float) (BaseSPD + GeneratedLevel));
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
