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

        #region Save
        public void SaveGame(GameContext context)
        {
            if (context == null) return; // if there is no hero, it means no game progress

            // Mapping
            var saveData = new Data
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
            // convert items to itemdata and append to player inventory json
            List<InventoryData> convertedInventory = new List<InventoryData>();

            if (context.Player.Inventory != null)
            {
                foreach (var item in context.Player.Inventory)
                {
                    var itemData = new InventoryData
                    {
                        ID = item.ID,
                        Quantity = item.Quantity,
                        Upgrade = item.Upgrade,
                        
                    };
                    convertedInventory.Add(itemData);
                }
            }

            saveData.Player.Inventory = convertedInventory;

            string jsonString = JsonConvert.SerializeObject(saveData, Newtonsoft.Json.Formatting.Indented);

            // Update the file
            File.WriteAllText(_savePath, jsonString);

            Debug.Log($"[SYSTEM] Game Saved.");
            Debug.Log($"[SYSTEM] Auto Save is {(saveData.IsAutoSaveOn ? "ENABLED" : "DISABLED")}.");

        }
        #endregion

        #region Load
        //public GameContext LoadGame()
        //{
        //    if (!File.Exists(_savePath))
        //    {
        //        Debug.LogWarning($"[SYSTEM] No Save file found.");
        //        Debug.Log($"[SYSTEM] Creating new save file.");

        //        var newContext = new GameContext();

        //        StaticData.LoadStaticDatas(newContext);

        //        newContext.Player = null;

        //        InitializeEvents(newContext);

        //        return newContext;
        //    }

        //    // Read File and cache it as a string
        //    string jsonString = File.ReadAllText(_savePath);
        //    // Convert to Data object
        //    Data? loadedData = JsonConvert.DeserializeObject<Data>(jsonString);
        //    // Convert to context so we can use it in the game
        //    var context = new GameContext();

        //    // Load Database to Cache
        //    StaticData.LoadStaticDatas(context);

        //    // Data Mapping
        //    DynamicData.LoadPlayerData(context, loadedData!);

        //    InitializeEvents(context);

        //    Debug.Log($"[SYSTEM] Game Loaded successfuly");

        //    return context;
        //}
        #endregion

        public GameContext LoadGame()
        {
            if (!File.Exists(_savePath))
            {
                Debug.LogWarning($"[SYSTEM] No save file Found. Creating new save file...");

                var newContext = new GameContext();

                StaticData.LoadStaticDatas(newContext);

                newContext.Player = null;

                InitializeEvents(newContext);

                return newContext;
            }

            try
            {
                string jsonString = File.ReadAllText(_savePath);
                Data loadedData = JsonConvert.DeserializeObject<Data>(jsonString);

                var context = new GameContext();
                StaticData.LoadStaticDatas(context);

                if (loadedData != null)
                {
                    DynamicData.LoadPlayerData(context, loadedData);
                }

                InitializeEvents(context);
                Debug.Log($"[SYSTEM] Game loaded successfully");
                return context;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[LOAD ERROR] There was a problem while loading the file: {e.Message}");
                return new GameContext();
            }
        }


        /// <summary>
        /// Subscribe to events
        /// </summary>
        /// <param name="context"></param>
        private static void InitializeEvents(GameContext context)
        {
            // clearing in case cuz defensive programming
            EventManager.HeroEvents.OnExpChanged -= (context) => LevelManager.CheckLevelUp(context);
            EventManager.HeroEvents.OnExpChanged += (context) => LevelManager.CheckLevelUp(context);
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
