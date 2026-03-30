////using System;
//using System;
//using System.Linq;
//using TextBasedRPG.Core.Entities;
//using TextBasedRPG.Core.Locations;
//using Random = UnityEngine.Random;

//namespace TextBasedRPG.Managers
//{
//    public static class EnemyGenerator
//    {
//        public static Entity GenerateEnemy(GameContext context)
//        {
//            string currentLocationId = context.Player?.ActiveLocation ?? "L001";
//            var availablePool = context.Entities?
//                .Where(e => e.ID.StartsWith("E") &&
//                           e.Locations != null &&
//                           e.Locations.Contains(currentLocationId))
//                .ToList();

//            if (availablePool == null || availablePool.Count == 0)
//            {
//                throw new Exception($"[DATA ERROR] No enemies found for location: {currentLocationId}. " +
//                                    "Check Entities.json for matching Location IDs.");
//            }

//            int randomIndex = Random.Range(0, availablePool.Count);
//            var template = availablePool[randomIndex];

//            Entity newEntity = new Enemy();

//            return MapEntityData(template, newEntity, context);
//        }
//        private static Entity MapEntityData(Entity template, Entity newEntity, GameContext context)
//        {
//            newEntity.ID = template.ID;
//            newEntity.Name = template.Name;
//            newEntity.BaseHP = template.BaseHP;
//            newEntity.BaseATK = template.BaseATK;
//            newEntity.BaseDEF = template.BaseDEF;
//            newEntity.Level = template.Level;
//            newEntity.Scaling = template.Scaling;
//            newEntity.EliteChance = template.EliteChance;
//            newEntity.LootTable = template.LootTable;
//            newEntity.GoldMultiplier = template.GoldMultiplier;
//            newEntity.EntityType = template.EntityType;

//            Location currentLocation = context.Locations!.FirstOrDefault(x => x.ID == context.Player!.ActiveLocation) ?? context.Locations![0];
//            newEntity.Initialize(playerLevel: context.Player!.Level, levelCap: currentLocation.LevelCap);

//            newEntity.Level = newEntity.GeneratedLevel;
//            return newEntity;
//        }
//    }
//}

using System.Linq;
using TextBasedRPG.Core.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TextBasedRPG.Managers
{
    public static class EnemyGenerator
    {
        public static Entity GenerateEnemy(GameContext context)
        {
            string currentLocationId = context.Player?.ActiveLocation ?? "L001";

            var availablePool = context.Entities?
                .Where(e => e.Locations != null && e.Locations.Contains(currentLocationId))
                .ToList();

            if (availablePool == null || availablePool.Count == 0)
            {
                Debug.LogError($"[DATA ERROR] No enemies for location: {currentLocationId}");
                return null;
            }

            Entity template = availablePool[Random.Range(0, availablePool.Count)];

            Entity newEnemy = new Enemy(template);

            var currentLocation = context.Locations?.FirstOrDefault(x => x.ID == currentLocationId);
            int levelCap = currentLocation?.LevelCap ?? 1000;

            newEnemy.Initialize(context.Player.Level, levelCap);

            return newEnemy;
        }
    }
}
