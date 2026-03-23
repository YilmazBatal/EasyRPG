using System.Collections.Generic;
using System.Linq;
using TextBasedRPG.Core.Entities;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Events;
using TextBasedRPG.Models;

namespace TextBasedRPG.Managers
{
    internal static class CombatManager
    {
        public static bool isCombatActive = true;

        public static void StartCombat(GameContext context)
        {
            Combat(context);
        }

        public static void Combat(GameContext context)
        {
            Entity enemy = EnemyGenerator.GenerateEnemy(context); // Generate the enemy
            List<string> log = new List<string>(); // Create the logs for the PvE

            // Who is faster?
            bool isPlayerTurn = context.Player?.TotalSPD >= enemy.CurrentSPD;
            bool isPlayerStarted = isPlayerTurn;

            isCombatActive = true;

            //while (isCombatActive)
            //{
            //    CombatUI.RefreshUI(context, enemy, log);

            //    // 1. Action Phase
            //    if (isPlayerTurn)
            //        CombatLogic.Acting(context, enemy, log);
            //    else
            //        CombatLogic.EnemyAct(context, enemy, log);

            //    // 2. Did anyone died after an action?
            //    if (context.Player?.CurHP <= 0 || enemy.CurHP <= 0)
            //    {
            //        isCombatActive = false;
            //        if (context.Player?.CurHP <= 0) {
            //            context.Player.ApplyDeathPenalty();
            //        }
            //        else
            //        {
            //            MenuUI.ColoredMsg(ConsoleColor.Green, "Enemy is dead");
            //            GiveLoot(context, enemy);
            //            Console.ReadKey(true);
            //            isCombatActive = false;
            //        }

            //        break;
            //    }

            //    // 3. The round has ended
            //    if (isPlayerTurn != isPlayerStarted)
            //    {
            //        EventManager.Combat.TriggerOnRoundEnded();
            //    }

            //    // 4. Switch Turns. if i add paralyze or stun , i can check here if the player or enemy is paralyzed or stunned and skip their turn
            //    isPlayerTurn = !isPlayerTurn;
            //}
        
        }
        //public static void OnHit(int damage, bool isCrit)
        //{
        //    Console.ForegroundColor = ConsoleColor.Red;
        //    string critText = isCrit ? "Critical hit!" : "";
        //    Console.WriteLine($"\nDealt {damage} damage! {critText} ");
        //    Console.ResetColor();
        //}
        //private static void GiveLoot(GameContext context, Entity enemy)
        //{
        //    // Gold & Exp
        //    double goldBase = enemy.PowerScore * enemy.GoldMultiplier * Math.Sqrt(enemy.Level);
        //    double randomMultiplier = 1 + (Random.Shared.NextDouble() * (0.15));
        //    int finalGold = (int)Math.Round(goldBase * randomMultiplier);
        //    context.Player.Gold += finalGold;
        //    MenuUI.ColoredMsg(ConsoleColor.Yellow, $"You received {finalGold} gold!");

        //    double levelDiffBonus = (enemy.Level > context.Player.Level) ? 1.2 : (enemy.Level < context.Player.Level ? 0.8 : 1.0);
        //    double expBase = (enemy.PowerScore * enemy.Level) / 5.0;
        //    int finalExp = (int)Math.Round(expBase * levelDiffBonus);
        //    context.Player.CurExp += finalExp;
        //    EventManager.HeroEvents.TriggerExpGained(finalExp);
        //    MenuUI.ColoredMsg(ConsoleColor.Cyan, $"You received {finalExp} experience!");

        //    // Items
        //    var currentMap = context.Locations.FirstOrDefault(l => l.ID == context.Player.ActiveLocation);

        //    if (currentMap?.AdventureLoots != null && currentMap.AdventureLoots.Count > 0)
        //    {
        //        int totalWeight = currentMap.AdventureLoots.Sum(x => x.DropChance);
        //        int roll = Random.Shared.Next(0, totalWeight);
        //        int currentWeight = 0;

        //        foreach (var loot in currentMap.AdventureLoots)
        //        {
        //            currentWeight += loot.DropChance;

        //            if (roll < currentWeight)
        //            {
        //                int amount = Random.Shared.Next(1, loot.MaxAmount + 1);

        //                var itemTemplate = FindItemByID(context, loot.ID);

        //                if (itemTemplate != null)
        //                {
        //                    InventoryManager.AddToInventory(context, loot, amount);
        //                    MenuUI.ColoredMsg(ConsoleColor.Green, $"You found {amount}x {itemTemplate.Name}!");
        //                }

        //                break;
        //            }
        //        }
        //    }

        //}

        private static Item? FindItemByID(GameContext context, string id)
        {
            if (id.StartsWith("W")) return context.Weapons?.FirstOrDefault(i => i.ID == id);
            if (id.StartsWith("A")) return context.Armors?.FirstOrDefault(i => i.ID == id);
            if (id.StartsWith("M")) return context.Materials?.FirstOrDefault(i => i.ID == id);
            if (id.StartsWith("C")) return context.Consumables?.FirstOrDefault(i => i.ID == id);

            return null;
        }
    }
}
