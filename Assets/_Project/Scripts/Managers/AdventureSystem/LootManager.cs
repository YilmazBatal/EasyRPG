using System.Linq;
using TextBasedRPG.Core.Entities;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Core.Locations;

namespace Assets._Project.Scripts.Managers.AdventureSystem
{
    public enum LootType
    {
        Entity,
        Adventure,
    }
    public class LootResult
    {
        public string ID;
        public int Amount;
    }
    public static class LootManager
    {
        public static LootResult AdventureLootGenerator(Location loc)
        {
            //a : 5, b : 3, c : 4 // total 12
            int totalWeight = 0;
            foreach (var loot in loc.AdventureLoots) totalWeight += loot.Weight;

            //dice = 7
            int randomRoll = (UnityEngine.Random.Range(0, totalWeight));
            
            // 5 + 3
            int currentSum = 0;
            foreach (var loot in loc.AdventureLoots)
            {
                currentSum += loot.Weight;
                if (randomRoll < currentSum)
                {
                    int droppedAmount = UnityEngine.Random.Range(1, loot.MaxAmount + 1);

                    return new LootResult { ID = loot.ID, Amount = droppedAmount };
                }
            }
            return null;
        }
        public static LootResult EnemyLootGenerator(Entity entity)
        {
            //a : 5, b : 3, c : 4 // total 12
            int totalWeight = 0;
            foreach (var loot in entity.Loots) totalWeight += loot.Weight;

            //dice = 7
            int randomRoll = (UnityEngine.Random.Range(0, totalWeight));

            // 5 + 3
            int currentSum = 0;
            foreach (var loot in entity.Loots)
            {
                currentSum += loot.Weight;
                if (randomRoll < currentSum)
                {
                    int droppedAmount = UnityEngine.Random.Range(1, loot.MaxAmount + 1);

                    return new LootResult { ID = loot.ID, Amount = droppedAmount };
                }
            }
            return null;
        }
        public static Item? FindItemByID(string id)
        {
            GameContext context = GameManager.Instance.Context;
            if (id.StartsWith("W")) return context.Weapons?.FirstOrDefault(i => i.ID == id);
            if (id.StartsWith("A")) return context.Armors?.FirstOrDefault(i => i.ID == id);
            if (id.StartsWith("M")) return context.Materials?.FirstOrDefault(i => i.ID == id);
            if (id.StartsWith("C")) return context.Consumables?.FirstOrDefault(i => i.ID == id);

            return null;
        }
    }
}
