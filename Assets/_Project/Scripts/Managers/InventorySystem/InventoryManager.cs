using System.Linq;
using TextBasedRPG.Events;
using TextBasedRPG.Models;

namespace TextBasedRPG.Managers
{
    internal class InventoryManager
    {
        public static void AddToInventory(GameContext context, Loots loot, int amount)
        {
            var existingItem = context.Player.Inventory!.FirstOrDefault(x => x.ID == loot.ID);
            if (existingItem != null)
            {
                existingItem.Quantity += amount;
            }
            else
            {
                InventoryData itemToAdd = new InventoryData();
                itemToAdd.ID = loot.ID;
                itemToAdd.Quantity = amount;

                context.Player.Inventory!.Add(itemToAdd);
                GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);
            }
        }

        /// <summary>
        /// Removes a given amount of an item from the player's inventory.
        /// If the remaining quantity drops to zero or below, the entry is removed entirely.
        /// Fires the EquipmentChanged event to refresh the UI afterwards.
        /// </summary>
        /// <returns>True if the item was found and discarded, false otherwise.</returns>
        public static bool RemoveFromInventory(GameContext context, string itemId, int amount)
        {
            var inventory = context.Player.Inventory;
            if (inventory == null) return false;

            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].ID == itemId)
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
