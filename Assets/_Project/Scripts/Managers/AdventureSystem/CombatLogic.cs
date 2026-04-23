using System;
using TextBasedRPG.Core.Entities;

namespace TextBasedRPG.Managers
{
    internal static class CombatLogic
    {
        /// <returns>Enemy's condition based on HP percentage</returns>
        public static string GetEnemyStatus(Entity enemy)
        {
            // ANSI
            string red = "\u001b[31m";
            string yellow = "\u001b[33m";
            string reset = "\u001b[0m"; // default color

            string coloredName = $"{red}{enemy.Name}{reset}";

            if (enemy.CurHP >= enemy.TotalHP * 75 / 100)
                return $"[BATTLE] {coloredName} looks ready to fight!";
            else if (enemy.CurHP >= enemy.TotalHP * 50 / 100)
                return $"[BATTLE] {coloredName} is watching you carefully";
            else if (enemy.CurHP >= enemy.TotalHP * 20 / 100)
                return $"[BATTLE] {coloredName} looks slightly tired to fight...";
            else
                return $"[BATTLE] {yellow}{enemy.Name}{reset} is panicking!";
        }
        private static int CalculateRunAwayChance(GameContext context, Entity entity)
        {
            int enemySpeed = entity.CurrentSPD;
            int playerSpeed = context.Player.TotalSPD;
            int baseLuck = 50; // %
            int luckMultiplier = 2;
            int runAwayChance = baseLuck + (playerSpeed - enemySpeed) * luckMultiplier;
            int runAwayChanceFinal = Math.Clamp(runAwayChance, 0, 100);
            return runAwayChanceFinal;
        }
    }
}
