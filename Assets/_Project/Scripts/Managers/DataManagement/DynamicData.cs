using System.Linq;
using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Models;
using UnityEngine;

namespace TextBasedRPG.Managers.DataManagement
{
    public static class DynamicData
    {
        public static void LoadPlayerData(GameContext context, Data loadedData)
        {
            if (loadedData != null)
            {
                LoadGeneralData(context, loadedData);
                LoadEquippedItems(context, loadedData);
                LoadInventory(context, loadedData);
                LoadInvestedStats(context,loadedData);
            }
        }
        private static void LoadGeneralData(GameContext context,Data loadedData)
        {
            context.IsAutoSaveOn = loadedData.IsAutoSaveOn;

            var heroDb = Resources.Load<HeroDatabase>("HeroDatabase");
            string className = loadedData.Player?.Class ?? "Warrior";
            HeroData data = heroDb.GetHeroByName(className);

            context.Player = className switch
            {
                "Warrior" => new Warrior(data),
                "Archer" => new Archer(data),
                "Mage" => new Mage(data),
                _ => new Warrior(data)
            }; // %100 can't be null

            context.Player.Gold = loadedData.Player?.Gold ?? 100;
            context.Player.Level = loadedData.Player?.Level ?? 1;
            context.Player.CurExp = loadedData.Player?.Experience ?? 0;
            context.Player.TotalExp = loadedData.Player?.TotalExp ?? 0;
            context.Player.CurHP = loadedData.Player?.CurHP ?? 1;
            context.Player.ActiveLocation = loadedData.Player?.ActiveLocation ?? "L001";
            context.Player.UnlockedUntill = loadedData.Player?.UnlockedUntill ?? 1;
            context.Player.Deaths = loadedData.Player?.Deaths ?? 0;
            context.Player.EntitiesSlayed = loadedData.Player?.EntitiesSlayed ?? 0;
            context.Player.HeaviestDamage = loadedData.Player?.HeaviestDamage ?? 0;
        }
        private static void LoadEquippedItems(GameContext context, Data loadedData)
        {
            string? savedWeaponID = loadedData.Player?.EquippedWeapon ?? null;
            string? savedArmorID = loadedData.Player?.EquippedArmor ?? null;
            context.Player.EquippedWeapon = string.IsNullOrEmpty(savedWeaponID)
                ? null : context.Weapons?.FirstOrDefault(x => x.ID == savedWeaponID);
            context.Player.EquippedArmor = string.IsNullOrEmpty(savedArmorID)
                ? null : context.Armors?.FirstOrDefault(x => x.ID == savedArmorID);
        }
        private static void LoadInventory(GameContext context, Data loadedData)
        {
            if (loadedData.Player?.Inventory != null)
            {
                context.Player.Inventory?.Clear();

                var allMasterItems = context.Weapons!.Cast<Item>()
                    .Concat(context.Armors!.Cast<Item>())
                    .Concat(context.Materials!.Cast<Item>())
                    .Concat(context.Consumables!.Cast<Item>())
                    .ToList();

                foreach (var itemSave in loadedData.Player.Inventory)
                {
                    var foundItem = allMasterItems.FirstOrDefault(i => i.ID == itemSave.ID);

                    if (foundItem != null)
                    {
                        InventoryData itemToAdd = new InventoryData();
                        itemToAdd.ID = itemSave.ID;
                        itemToAdd.Quantity = itemSave.Quantity;
                        context.Player.Inventory?.Add(itemToAdd);
                    }
                }
            }
        }
        private static void LoadInvestedStats(GameContext context, Data loadedData)
        {
            context.Player.UnusedStatPoints = loadedData.Player?.Stats?.UnusedStatPoints ?? 0;
            context.Player.InvestedSTRPoints = loadedData.Player?.Stats?.InvestedSTR ?? 0;
            context.Player.InvestedVITPoints = loadedData.Player?.Stats?.InvestedVIT ?? 0;
            context.Player.InvestedDEXPoints = loadedData.Player?.Stats?.InvestedDEX ?? 0;
            context.Player.InvestedAGIPoints = loadedData.Player?.Stats?.InvestedAGI ?? 0;
        }
    }
}
