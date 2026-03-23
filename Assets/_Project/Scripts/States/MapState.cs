public class MapState : IMenuState
{
    public GameState Update(GameContext context)
    {
        //Console.Clear();
        MapPagination(context);
        return GameState.MainMenu;
    }

    public static void MapPagination(GameContext context)
    {
        //var locations = context.Locations;
        //if (locations == null) return;

        //int pageCount = (int)Math.Ceiling((double)locations.Count / MenuUI.ITEMS_PER_PAGE);
        //int currentPage = 0;
        //bool inMenu = true;
        //string isItUnlocked;

        //while (inMenu)
        //{
        //    Console.Clear();
        //    MenuUI.MapTitle();
        //    MenuUI.ActiveLocationData(context);
        //    Console.WriteLine($"══════════════════ LOCATIONS PAGE {currentPage + 1} / {pageCount} ══════════════════");
        //    Console.WriteLine();
        //    Console.WriteLine($"    {"Location No",-12} {"Location Name",-25} {"State",-10}");
        //    Console.WriteLine("──────────────────────────────────────────────────────────");

        //    for (int j = 0; j < MenuUI.ITEMS_PER_PAGE; j++)
        //    {
        //        int currentIndex = j + (currentPage * MenuUI.ITEMS_PER_PAGE);

        //        if (currentIndex >= locations.Count)
        //            break;

        //        var item = locations[currentIndex];
        //        // 2 <= 3
        //        if (context.Player.UnlockedUntill < currentIndex + 1)
        //        {
        //            Console.ForegroundColor = ConsoleColor.DarkGray;
        //            Console.Write($"[{j + 1}] {item.ID,-12} {item.Name.ToString(),-25} {"Locked",-10} ");
        //            Console.ResetColor();
        //        }
        //        else
        //        {
        //            Console.ForegroundColor = ConsoleColor.Gray;
        //            Console.Write($"[{j + 1}] {item.ID,-12} {item.Name.ToString(),-25} {"Unlocked",-10} ");
        //        }
        //        Console.WriteLine();
        //    }
        //    Console.WriteLine("──────────────────────────────────────────────────────────");
        //    Console.WriteLine($"[P]revious | [N]ext | [B]ack");
        //    Console.WriteLine("══════════════════════════════════════════════════════════");
        //    Console.Write("Selection: ");

        //    string input = Console.ReadLine()?.ToUpper() ?? "";

        //    if (input == "N")
        //    {
        //        if (currentPage < pageCount - 1) currentPage++;
        //    }
        //    else if (input == "P")
        //    {
        //        if (currentPage > 0) currentPage--;
        //    }
        //    else if (input == "B") inMenu = false;
        //    else // selected number
        //    {
        //        if (int.TryParse(input, out int selection) && selection >= 1 && selection <= 9)
        //        {
        //            int realIndex = (currentPage * MenuUI.ITEMS_PER_PAGE) + (selection - 1);
        //            if (context.Player?.UnlockedUntill >= selection) // if unlocked
        //            {
        //                // selection = 1, 2 etc
        //                context.Player?.ActiveLocation = context.Locations?[realIndex].ID;
        //                MenuUI.ColoredMsg(ConsoleColor.Green, $"[SYSTEM] Successfully traveled to {context.Locations?[realIndex].Name}.");
        //                Thread.Sleep(750);
        //            }
        //            else
        //            {
        //                MenuUI.ColoredMsg(ConsoleColor.Red, "[SYSTEM] You have not unlocked this place yet.");
        //                Thread.Sleep(750);
        //            }
        //        }
        //        else
        //        {
        //            MenuUI.ColoredMsg(ConsoleColor.Red, "[SYSTEM] Invalid Input.");
        //            Thread.Sleep(750);
        //        }
        //    }
        //}
    }

}