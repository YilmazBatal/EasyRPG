using System.Linq;
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
            }
        }
        //public static void DiscardFromInventory(GameContext context, InventoryData invData)
        //{
        //    Console.ForegroundColor = ConsoleColor.Yellow;
        //    Console.Write("[INVENTORY] How many do you want to discard? > ");
        //    string amount = Console.ReadLine()!;
        //    if (int.TryParse(amount, out int result) && result > 0 && result <= invData.Quantity)
        //    {
        //        Console.Write($"\n[INVENTORY] Are you sure you want to discard? [Y/Any Key] > ");
        //        string selection = Console.ReadLine()!.ToUpper();
        //        if (selection == "Y")
        //        {
        //            invData.Quantity -= result;

        //            if (invData.Quantity <= 0)
        //            {
        //                context.Player!.Inventory!.Remove(invData);
        //            }

        //            Console.ForegroundColor = ConsoleColor.Red;
        //            Console.Write($"\n[INVENTORY] {invData.Quantity}x {context.MasterItemBook[invData.ID].Name} is discarded!");
        //            Console.ResetColor();
        //            Console.ReadKey(true);

        //        }
        //        else
        //        {
        //            MenuUI.ColoredMsg(ConsoleColor.Cyan, $"[INVENTORY] You have canceled the operation. ");
        //            Console.ReadKey(true);
        //        }
        //    }
        //    else
        //    {
        //        MenuUI.ColoredMsg(ConsoleColor.Yellow, "[INVENTORY] Invalid input.");
        //        Console.ReadKey(true);
        //    }
        //}
    
    }
}
