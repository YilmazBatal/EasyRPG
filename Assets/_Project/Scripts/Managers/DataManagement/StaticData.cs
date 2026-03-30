using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using TextBasedRPG.Core.Entities;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Core.Locations;
using TextBasedRPG.Core.Shops;
using TextBasedRPG.Models;
using UnityEngine;
using Material = TextBasedRPG.Core.Items.Material;

namespace TextBasedRPG.Managers.DataManagement
{
    public static class StaticData
    {
        private const string EntitiesFile = "Entities.json";
        private const string LocationsFile = "Locations.json";
        private const string WeaponsFile = "Weapons.json";
        private const string ArmorsFile = "Armors.json";
        private const string MaterialsFile = "Materials.json";
        private const string ConsumablesFile = "Consumables.json";
        private const string ShopsFile = "Shops.json";

        private static string GetStaticPath(string fileName)
        {
            return Path.Combine(Application.streamingAssetsPath, fileName);
        }
        public static void LoadStaticDatas(GameContext context)
        {
            LoadLocations(context);
            LoadEntities(context);
            LoadWeapons(context);
            LoadArmors(context);
            LoadMaterials(context);
            LoadConsumables(context);
            LoadShops(context);
            context.InitializeMasterBook();
            context.InitializeClassWeaponCheck();
        }

        private static void LoadLocations(GameContext context)
        {
            string path = GetStaticPath(LocationsFile);

            if (!File.Exists(path))
            {
                //MenuUI.ColoredMsg(ConsoleColor.Red, $"[ERROR] File not found! Path: {Path.GetFullPath(path)}");
                Debug.LogWarning($"[ERROR] File not found! Path: {Path.GetFullPath(path)}");
                context.Locations = new List<Location>();
                return;
            }

            string jsonString = File.ReadAllText(path);

            List<LocationData>? loadedData = JsonConvert.DeserializeObject<List<LocationData>>(jsonString);

            if (loadedData != null)
            {
                context.Locations = new List<Location>();
                foreach (var location in loadedData)
                {
                    Location mappedLocation = new Location(
                        location.ID,
                        location.Name,
                        description: location.Description,
                        levelCap: location.LevelCap,
                        texts: location.AdventureTexts,
                        loots: location.AdventureLoots,
                        entities: location.Entities
                    );
                    context.Locations?.Add(mappedLocation);
                }

                LocationManager.LocationMapping(context);
            }
        }
        private static void LoadEntities(GameContext context)
        {
            string path = GetStaticPath(EntitiesFile);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[ERROR] File not found! Path: {Path.GetFullPath(path)}");
                context.Entities = new List<Entity>();
                return;
            }

            string jsonString = File.ReadAllText(path);

            List<MobData>? loadedData = JsonConvert.DeserializeObject<List<MobData>>(jsonString);

            if (loadedData != null)
            {
                context.Entities = new List<Entity>();
                foreach (var data in loadedData)
                {
                    Entity mappedEntity;
                    if (data.EntityType == "Boss")
                        mappedEntity = new Boss();
                    else
                        mappedEntity = new Enemy();

                    mappedEntity.ID = data.ID;
                    mappedEntity.Name = data.Name;
                    mappedEntity.BaseHP = data.BaseHP;
                    mappedEntity.BaseATK = data.BaseATK;
                    mappedEntity.BaseDEF = data.BaseDEF;
                    mappedEntity.BaseSPD = data.BaseSPD;
                    mappedEntity.EntitySprite = data.EntitySprite;
                    mappedEntity.GoldMultiplier = data.GoldMultiplier;
                    mappedEntity.LootTable = data.LootTable ?? new();
                    mappedEntity.Locations = data.Locations ?? new();
                    if (Enum.TryParse<EntityType>(data.EntityType.ToString(), true, out var type))
                    {
                        mappedEntity.EntityType = type;
                    }
                    else
                    {
                        mappedEntity.EntityType = EntityType.Enemy;
                    }

                    context.Entities?.Add(mappedEntity);
                }
            }
        }
        private static void LoadWeapons(GameContext context)
        {
            string path = GetStaticPath(WeaponsFile);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[ERROR] File not found! Path: {Path.GetFullPath(path)}");
                context.Weapons = new List<Weapon>();
                return;
            }

            string jsonString = File.ReadAllText(path);

            List<WeaponData> loadedData = JsonConvert.DeserializeObject<List<WeaponData>>(jsonString);

            if (loadedData != null)
            {
                context.Weapons = new List<Weapon>();
                foreach (var data in loadedData)
                {
                    Enum.TryParse<ItemType>(data.ItemType, true, out var itemTypeResult);
                    Enum.TryParse<Rarity>(data.Rarity, true, out var rarityResult);
                    Enum.TryParse<WeaponType>(data.WeaponType, true, out var weaponTypeResult);

                    Weapon mappedWeapon = new Weapon(
                        data.ID,
                        itemTypeResult,
                        data.Name ?? "Unknown Item",
                        data.Description,
                        data.Price,
                        rarityResult,
                        data.Quantity,
                        data.WeaponATK,
                        weaponTypeResult,
                        data.RequiredLevel
                    );
                    context.Weapons.Add(mappedWeapon);
                }
            }
        }
        private static void LoadArmors(GameContext context)
        {
            string path = GetStaticPath(ArmorsFile);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[ERROR] File not found! Path: {Path.GetFullPath(path)}");
                context.Armors = new List<Armor>();
                return;
            }

            string jsonString = File.ReadAllText(path);

            List<ArmorData>? loadedData = JsonConvert.DeserializeObject<List<ArmorData>>(jsonString);

            if (loadedData != null)
            {
                context.Armors = new List<Armor>();
                foreach (var data in loadedData)
                {
                    Enum.TryParse<ItemType>(data.ItemType, true, out var itemTypeResult);
                    Enum.TryParse<Rarity>(data.Rarity, true, out var rarityResult);

                    Armor mappedArmor = new Armor(
                        data.ID,
                        itemTypeResult,
                        data.Name ?? "Unknown Item",
                        data.Description,
                        data.Price,
                        rarityResult,
                        data.Quantity,
                        data.ArmorDef,
                        data.ExtraHP,
                        data.RequiredLevel
                    );
                    context.Armors.Add(mappedArmor);
                }
            }
        }
        private static void LoadMaterials(GameContext context)
        {
            string path = GetStaticPath(MaterialsFile);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[ERROR] File not found! Path: {Path.GetFullPath(path)}");
                context.Materials = new List<Material>();
                return;
            }

            string jsonString = File.ReadAllText(path);

            List<MaterialData>? loadedData = JsonConvert.DeserializeObject<List<MaterialData>>(jsonString);

            if (loadedData != null)
            {
                context.Materials = new List<Material>();
                foreach (var data in loadedData)
                {
                    Enum.TryParse<ItemType>(data.ItemType, true, out var itemTypeResult);
                    Enum.TryParse<Rarity>(data.Rarity, true, out var rarityResult);

                    Material mappedMaterial = new Material(
                        data.ID,
                        itemTypeResult,
                        data.Name ?? "Unknown Item",
                        data.Description,
                        data.Price,
                        rarityResult,
                        data.Quantity,
                        data.MaxQuantity
                    );
                    context.Materials.Add(mappedMaterial);
                }
            }
        }
        private static void LoadConsumables(GameContext context)
        {
            string path = GetStaticPath(ConsumablesFile);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[ERROR] File not found! Path: {Path.GetFullPath(path)}");
                context.Consumables = new List<Consumable>();
                return;
            }

            string jsonString = File.ReadAllText(path);

            List<ConsumableData>? loadedData = JsonConvert.DeserializeObject<List<ConsumableData>>(jsonString);

            if (loadedData != null)
            {
                context.Consumables = new List<Consumable>();
                foreach (var data in loadedData)
                {
                    Enum.TryParse<ItemType>(data.ItemType, true, out var itemTypeResult);
                    Enum.TryParse<Rarity>(data.Rarity, true, out var rarityResult);

                    Consumable mappedConsumable = new Consumable(
                        data.ID,
                        itemTypeResult,
                        data.Name ?? "Unknown Item",
                        data.Description,
                        data.Price,
                        rarityResult,
                        data.Quantity,
                        data.Effect ?? "HP",
                        data.Value,
                        data.CombatItem
                    );
                    context.Consumables.Add(mappedConsumable);
                }
            }
        }
        private static void LoadShops(GameContext context)
        {
            string path = GetStaticPath(ShopsFile);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"[ERROR] File not found! Path: {Path.GetFullPath(path)}");
                context.Shops = new List<Shop>();
                return;
            }

            string jsonString = File.ReadAllText(path);

            List<ShopData>? loadedData = JsonConvert.DeserializeObject<List<ShopData>>(jsonString);

            if (loadedData != null)
            {
                context.Shops = new List<Shop>();
                foreach (var shop in loadedData)
                {
                    Shop mappedShop = new Shop(
                         shop.ID,
                         shop.ShopName,
                         shop.Items
                    );
                    context.Shops?.Add(mappedShop);
                }
            }
        }
    }
}
