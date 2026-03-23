using System;
using Random = UnityEngine.Random;

namespace TextBasedRPG.Core.Entities
{
    internal class Enemy : Entity
    {
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
