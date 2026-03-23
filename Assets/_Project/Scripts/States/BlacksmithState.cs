public class BlacksmithState : IMenuState
{
    //private static readonly Dictionary<string, string> LocationToShopMap = new()
    //{
    //    { "L001", "S001" },
    //    { "L002", "S002" }, 
    //    { "L003", "S003" } 
    //};
    public GameState Update(GameContext context)
    {
        //Console.Clear();

        //if (LocationToShopMap.TryGetValue(context.Player.ActiveLocation, out string targetShopID))
        //{
        //    var currentShop = context.Shops.FirstOrDefault(s => s.ID == targetShopID);

        //    if (currentShop != null)
        //    {
        //        ShopPagination(context, currentShop);
        //        return GameState.MainMenu;
        //    }
        //    else
        //    {
        //        MenuUI.ColoredMsg(ConsoleColor.Red, "[SYSTEM] Couldn't load the shop.");
        //    }

        //}
        return GameState.MainMenu;
    }
    //public void ShopPagination(GameContext context, Shop shop)
    //{
    //    var itemIDs = shop.Items; // Dükkandaki string ID listesi
    //    if (itemIDs == null || itemIDs.Count == 0) return;

    //    int pageCount = (int)Math.Ceiling((double)itemIDs.Count / MenuUI.ITEMS_PER_PAGE);
    //    int currentPage = 0;
    //    bool inMenu = true;

    //    while (inMenu)
    //    {
    //        Console.Clear();
    //        MenuUI.ShopTitle();
    //        Console.WriteLine($" Welcome to: {shop.ShopName}");
    //        Console.WriteLine($" Your Gold: {context.Player.Gold}");
    //        Console.WriteLine($"═════════════════════ ITEMS {currentPage + 1} / {pageCount} ═════════════════════");
    //        Console.WriteLine();
    //        Console.WriteLine($"    {"Name",-20} {"Type",-12} {"Rarity",-15} {"Price",-10}");
    //        Console.WriteLine("────────────────────────────────────────────────────────────────");

    //        for (int j = 0; j < MenuUI.ITEMS_PER_PAGE; j++)
    //        {
    //            int currentIndex = j + (currentPage * MenuUI.ITEMS_PER_PAGE);
    //            if (currentIndex >= itemIDs.Count) break;

    //            string itemID = itemIDs[currentIndex];

    //            // MasterItemBook üzerinden eşya detaylarını çekiyoruz
    //            if (context.MasterItemBook.TryGetValue(itemID, out var item))
    //            {
    //                Console.Write($"[{j + 1}] {item.Name,-20} {item.ItemType,-12} ");
    //                MenuUI.SetRarityColor(item.Rarity.ToString());
    //                Console.Write($"{item.Rarity,-15}");
    //                Console.ResetColor();
    //                MenuUI.ColoredMsg(ConsoleColor.DarkYellow, $"{item.Price + " G",-10}");
    //            }
    //        }

    //        Console.WriteLine("────────────────────────────────────────────────────────────────");
    //        Console.WriteLine($"[P]revious | [N]ext | [B]ack");
    //        Console.WriteLine("════════════════════════════════════════════════════════════════");
    //        Console.Write("Selection: ");

    //        string input = Console.ReadLine()?.ToUpper() ?? "";

    //        if (input == "N" && currentPage < pageCount - 1) currentPage++;
    //        else if (input == "P" && currentPage > 0) currentPage--;
    //        else if (input == "B") inMenu = false;
    //        else if (int.TryParse(input, out int selection) && selection >= 1 && selection <= MenuUI.ITEMS_PER_PAGE)
    //        {
    //            int realIndex = (currentPage * MenuUI.ITEMS_PER_PAGE) + (selection - 1);
    //            if (realIndex < itemIDs.Count)
    //            {
    //                // Satın alma işlemini başlat
    //                ProcessPurchase(context, itemIDs[realIndex]);
    //            }
    //        }
    //    }
    //}
    //private void ProcessPurchase(GameContext context, string itemID)
    //{
    //    if (!context.MasterItemBook.TryGetValue(itemID, out var item)) return;

    //    if (context.Player.Gold >= item.Price)
    //    {
    //        context.Player.Gold -= item.Price;

    //        var existing = context.Player.Inventory.FirstOrDefault(x => x.ID == itemID);
    //        if (existing != null) existing.Quantity++;
    //        else context.Player.Inventory.Add(new InventoryData { ID = itemID, Quantity = 1 });

    //        MenuUI.ColoredMsg(ConsoleColor.Green, $"[SYSTEM] Bought {item.Name} for {item.Price} gold!");
    //    }
    //    else
    //    {
    //        MenuUI.ColoredMsg(ConsoleColor.Red, "[SYSTEM] You don't have enough gold!");
    //    }
    //    Thread.Sleep(800);
    //}

}