using Assets._Project.Scripts.Managers.AdventureSystem;
using System;
using System.Linq;
using TextBasedRPG.Events;
using TextBasedRPG.Models;
using UnityEngine;

namespace TextBasedRPG.Managers
{
    public static class InventoryManager
    {
        public static void AddToInventory(string itemId, int amount = 1)
        {
            AddToInventory(new LootResult { ID = itemId, Amount = amount });
        }

        public static void AddToInventory(LootResult loot)
        {
            if (loot == null || string.IsNullOrEmpty(loot.ID)) return;

            if (GameManager.Instance == null || GameManager.Instance.Context?.Player == null)
            {
                Debug.LogError("[INVENTORY] Cannot add item: GameManager or Player context is null.");
                return;
            }

            GameContext context = GameManager.Instance.Context;
            context.Player.Inventory ??= new System.Collections.Generic.List<InventoryData>();

            char itemTypeID = loot.ID[0];
            bool isStackable = (itemTypeID != 'W' && itemTypeID != 'A');

            if (isStackable)
            {
                var existingStack = context.Player.Inventory.FirstOrDefault(x => x != null && x.ID == loot.ID);

                if (existingStack != null)
                {
                    existingStack.Quantity += loot.Amount;
                    Debug.Log($"[INVENTORY] Updated stack for {loot.ID}, new qty: {existingStack.Quantity}");

                    EventManager.HeroEvents.TriggerEquipmentChanged(context);
                    GameManager.Instance.SaveService?.SaveGame(context);
                    return;
                }
            }

            int countToCreate = isStackable ? 1 : Mathf.Max(1, loot.Amount);
            int singleAmount = isStackable ? loot.Amount : 1;

            for (int i = 0; i < countToCreate; i++)
            {
                CreateNewInventoryEntry(context, loot.ID, singleAmount);
            }
        }

        private static void CreateNewInventoryEntry(GameContext context, string itemId, int amount)
        {
            InventoryData itemToAdd = new InventoryData
            {
                InstanceID = Guid.NewGuid().ToString(),
                ID = itemId,
                Quantity = amount,
                Upgrade = 0
            };

            context.Player.Inventory!.Add(itemToAdd);
            Debug.Log($"[INVENTORY] Added new item entry: {itemId} (x{amount})");

            EventManager.HeroEvents.TriggerEquipmentChanged(context);
            GameManager.Instance.SaveService?.SaveGame(context);
        }

        public static bool RemoveFromInventory(string itemId, int amount = 1)
        {
            if (GameManager.Instance == null || GameManager.Instance.Context?.Player?.Inventory == null) return false;

            GameContext context = GameManager.Instance.Context;
            var inventory = context.Player.Inventory;

            char itemTypeID = string.IsNullOrEmpty(itemId) ? ' ' : itemId[0];
            bool isStackable = (itemTypeID != 'W' && itemTypeID != 'A');

            if (isStackable)
            {
                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i] != null && inventory[i].ID == itemId)
                    {
                        inventory[i].Quantity -= amount;

                        if (inventory[i].Quantity <= 0)
                            inventory.RemoveAt(i);

                        EventManager.HeroEvents.TriggerEquipmentChanged(context);
                        GameManager.Instance.SaveService?.SaveGame(context);
                        return true;
                    }
                }
            }
            else
            {
                var entry = inventory.FirstOrDefault(x => x != null && x.ID == itemId);
                if (entry != null)
                {
                    inventory.Remove(entry);
                    EventManager.HeroEvents.TriggerEquipmentChanged(context);
                    GameManager.Instance.SaveService?.SaveGame(context);
                    return true;
                }
            }

            return false;
        }
    }
}

