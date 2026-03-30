using System;
using Random = UnityEngine.Random;

namespace TextBasedRPG.Core.Entities
{
    internal class Enemy : Entity
    {
        public Enemy() { }

        public Enemy(Entity template)
        {
            this.ID = template.ID;
            this.Name = template.Name;
            this.BaseHP = template.BaseHP;
            this.BaseATK = template.BaseATK;
            this.BaseDEF = template.BaseDEF;
            this.BaseSPD = template.BaseSPD;
            this.EntitySprite = template.EntitySprite;
            this.Level = template.Level;
            this.Scaling = template.Scaling;
            this.EliteChance = template.EliteChance;
            this.LootTable = template.LootTable;
            this.GoldMultiplier = template.GoldMultiplier;
            this.EntityType = template.EntityType;
            this.Locations = template.Locations;
        }
        public override void Initialize(int playerLevel, int regionCap)
        {
            isElite = Random.Range(0, 100) < EliteChance;
            int enemyLevel = Random.Range(playerLevel - LevelInterval, playerLevel + LevelInterval + 1);
            if (enemyLevel > regionCap)
                enemyLevel = regionCap;
            GeneratedLevel = Math.Max(1, enemyLevel);
            CurHP = TotalHP;
        }
    }
}
