using Assets._Project.Scripts.Managers.AdventureSystem;
using System.Linq;
using TextBasedRPG.Events;
using TextBasedRPG.Models;

namespace TextBasedRPG.Managers
{
    internal class InventoryManager
    {
        public static void AddToInventory(LootResult loot)
        {
            if (loot == null || string.IsNullOrEmpty(loot.ID)) return;

            if (GameManager.Instance == null)
            {
                UnityEngine.Debug.LogError("[INVENTORY] GameManager.Instance is null! (Are you calling this after the game stopped or before Awake?)");
                return;
            }

            GameContext context = GameManager.Instance.Context;
            if (context == null)
            {
                UnityEngine.Debug.LogError("[INVENTORY] GameManager.Instance.Context is null!");
                return;
            }

            if (context.Player == null)
            {
                UnityEngine.Debug.LogError("[INVENTORY] GameManager.Instance.Context.Player is null! (Are you trying to add an item before selecting a hero?)");
                return;
            }

            if (context.Player.Inventory == null)
            {
                context.Player.Inventory = new System.Collections.Generic.List<InventoryData>();
            }

            char itemTypeID = loot.ID[0];

            bool isStackable = (itemTypeID != 'W' && itemTypeID != 'A');

            if (isStackable)
            {
                var existingStack = context.Player.Inventory!.FirstOrDefault(x => x != null && x.ID == loot.ID);

                if (existingStack != null)
                {
                    existingStack.Quantity += loot.Amount;
                    UnityEngine.Debug.Log($"[INVENTORY] Updated stack for: {loot.ID}, new qty: {existingStack.Quantity}");
                }
                else
                {
                    CreateNewInventoryEntry(context, loot);
                }
            }
            else
            {
                CreateNewInventoryEntry(context, loot);
            }
        }

        private static void CreateNewInventoryEntry(GameContext context, LootResult loot)
        {
            InventoryData itemToAdd = new InventoryData
            {
                ID = loot.ID,
                Quantity = loot.Amount, 
                Upgrade = 0
            };

            context.Player.Inventory!.Add(itemToAdd);
            UnityEngine.Debug.Log($"[INVENTORY] Added new item: {loot.ID}");

            GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);
        }

        /// <summary>
        /// Removes a given amount of an item from the player's inventory.
        /// If the remaining quantity drops to zero or below, the entry is removed entirely.
        /// Fires the EquipmentChanged event to refresh the UI afterwards.
        /// </summary>
        /// <returns>True if the item was found and discarded, false otherwise.</returns>
        public static bool RemoveFromInventory(string itemId, int amount)
        {
            GameContext context = GameManager.Instance.Context;
            var inventory = context.Player.Inventory;
            if (inventory == null) return false;

            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] != null && inventory[i].ID == itemId)
                {
                    inventory[i].Quantity -= amount;

                    if (inventory[i].Quantity <= 0)
                        inventory.RemoveAt(i);

                    EventManager.HeroEvents.TriggerEquipmentChanged(context);
                    GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);

                    return true;
                }
            }

            return false;
        }
    }
}
