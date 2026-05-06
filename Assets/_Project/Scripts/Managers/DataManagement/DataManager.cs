using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Core.Locations;
using TextBasedRPG.Events;
using TextBasedRPG.Models;
using UnityEngine;
using Material = TextBasedRPG.Core.Items.Material;

namespace  TextBasedRPG.Managers.DataManagement
{
    internal class DataManager : ISaveService
    {
        private readonly string _savePath = Path.Combine(Application.persistentDataPath, "save.json");
        private readonly string _webSaveKey = "TextBasedRPG_SaveData";

        public void SaveGame(GameContext context)
        {
            if (context == null) return;

            var saveData = MapContextToData(context);
            string jsonString = JsonConvert.SerializeObject(saveData, Newtonsoft.Json.Formatting.Indented);

            #if UNITY_WEBGL && !UNITY_EDITOR
                PlayerPrefs.SetString(_webSaveKey, jsonString);
                PlayerPrefs.Save();
            #else
                File.WriteAllText(_savePath, jsonString);
            #endif
        }

        public GameContext LoadGame()
        {
            bool saveExists = false;

            #if UNITY_WEBGL && !UNITY_EDITOR
                saveExists = PlayerPrefs.HasKey(_webSaveKey);
            #else
                saveExists = File.Exists(_savePath);
            #endif

            if (!saveExists)
            {
                return CreateNewContext();
            }

            try
            {
                string jsonString = "";

                #if UNITY_WEBGL && !UNITY_EDITOR
                    jsonString = PlayerPrefs.GetString(_webSaveKey);
                #else
                    jsonString = File.ReadAllText(_savePath);
                #endif

                Data loadedData = JsonConvert.DeserializeObject<Data>(jsonString);
                var context = new GameContext();
                StaticData.LoadStaticDatas(context);

                if (loadedData != null)
                {
                    DynamicData.LoadPlayerData(context, loadedData);
                }

                InitializeEvents(context);
                return context;
            }
            catch (System.Exception)
            {
                return CreateNewContext();
            }
        }

        private Data MapContextToData(GameContext context)
        {
            var data = new Data
            {
                IsAutoSaveOn = context.IsAutoSaveOn,
                Player = new Player
                {
                    Class = context.Player.ClassName,
                    Level = context.Player.Level,
                    ActiveLocation = context.Player.ActiveLocation,
                    UnlockedUntill = context.Player.UnlockedUntill,
                    Experience = context.Player.CurExp,
                    TotalExp = context.Player.TotalExp,
                    CurHP = context.Player.CurHP,
                    Gold = context.Player.Gold,
                    Deaths = context.Player.Deaths,
                    EntitiesSlayed = context.Player.EntitiesSlayed,
                    HeaviestDamage = context.Player.HeaviestDamage,
                    EquippedWeapon = context.Player.EquippedWeapon != null ? new EquippedWeaponData { ID = context.Player.EquippedWeapon.ID, Upgrade = context.Player.EquippedWeapon.Upgrade } : null,
                    EquippedArmor = context.Player.EquippedArmor != null ? new EquippedArmorData { ID = context.Player.EquippedArmor.ID, Upgrade = context.Player.EquippedArmor.Upgrade } : null,
                    Stats = new StatData
                    {
                        UnusedStatPoints = context.Player.UnusedStatPoints,
                        InvestedSTR = context.Player.InvestedSTRPoints,
                        InvestedVIT = context.Player.InvestedVITPoints,
                        InvestedDEX = context.Player.InvestedDEXPoints,
                        InvestedAGI = context.Player.InvestedAGIPoints,
                    }
                }
            };

            List<InventoryData> convertedInventory = new List<InventoryData>();
            if (context.Player.Inventory != null)
            {
                foreach (var item in context.Player.Inventory)
                {
                    convertedInventory.Add(new InventoryData
                    {
                        ID = item.ID,
                        Quantity = item.Quantity,
                        Upgrade = item.Upgrade
                    });
                }
            }
            data.Player.Inventory = convertedInventory;
            return data;
        }

        private GameContext CreateNewContext()
        {
            var newContext = new GameContext();

            StaticData.LoadStaticDatas(newContext);
            DynamicData.LoadPlayerData(newContext, null);

            InitializeEvents(newContext);

            return newContext;
        }

        private static void InitializeEvents(GameContext context)
        {
            EventManager.HeroEvents.OnExpChanged -= (ctx) => LevelManager.CheckLevelUp(ctx);
            EventManager.HeroEvents.OnExpChanged += (ctx) => LevelManager.CheckLevelUp(ctx);
        }

    }
    public class Data
    {
        public Player? Player { get; set; }
        public bool IsAutoSaveOn { get; set; } = true;
        //[JsonIgnore]
        public List<MobData>? EntityList { get; set; }
        //[JsonIgnore]
        public List<Location>? Locations { get; set; } 
        //[JsonIgnore]
        public List<Weapon>? Weapons { get; set; }
        //[JsonIgnore]
        public List<Armor>? Armors { get; set; }
        //[JsonIgnore]
        public List<Material>? Materials { get; set; }
        //[JsonIgnore]
        public List<Consumable>? Consumables { get; set; }
    }
}
