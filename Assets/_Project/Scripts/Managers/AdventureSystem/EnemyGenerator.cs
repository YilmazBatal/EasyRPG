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
