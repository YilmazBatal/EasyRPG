public class TrainingState : IMenuState
{
    public GameState Update(GameContext context)
    {
        //Console.Clear();
        //var p = context.Player;
        //int totalPointsSpent = p.InvestedSTRPoints + p.InvestedVITPoints + p.InvestedDEXPoints + p.InvestedAGIPoints;

        //#region Title
        //Console.ForegroundColor = ConsoleColor.Cyan;
        //Console.WriteLine("╔══════════════════════════════════════════════════════════╗");
        //Console.Write("║"); 
        //Console.ForegroundColor = ConsoleColor.White;
        //Console.Write("                WELCOME TO TRAINING GROUNDS               ");
        //Console.ForegroundColor = ConsoleColor.Cyan;
        //Console.WriteLine("║");
        //Console.WriteLine("╚══════════════════════════════════════════════════════════╝");
        //Console.ResetColor();
        //#endregion

        //#region Stats Table
        //Console.ForegroundColor = ConsoleColor.DarkGray;
        //Console.WriteLine("  ┌──────────┬──────────┬──────────┬──────────┐");
        //Console.Write("  │ ");
        //WriteStat("STR", p.InvestedSTRPoints);
        //WriteStat("VIT", p.InvestedVITPoints);
        //WriteStat("DEX", p.InvestedDEXPoints);
        //WriteStat("AGI", p.InvestedAGIPoints);
        //Console.WriteLine();
        //Console.ForegroundColor = ConsoleColor.DarkGray;
        //Console.WriteLine("  └──────────┴──────────┴──────────┴──────────┘");
        //Console.WriteLine();
        //Console.ResetColor();
        //#endregion

        //#region Stat Ivenstment
        //MenuUI.ColoredMsg(ConsoleColor.White, "──────────────────────────────────────────────────────────");
        //Console.Write("Unused Points: ");
        //MenuUI.ColoredMsg(ConsoleColor.Yellow, $"{p.UnusedStatPoints}");
        //Console.Write("Total Points: ");
        //MenuUI.ColoredMsg(ConsoleColor.White, $"{totalPointsSpent}");
        //Console.WriteLine();
        //MenuUI.MenuOption("1", "STR", "Increases attack & critical damage.");
        //MenuUI.MenuOption("2", "VIT", "Increases defence & health.");
        //MenuUI.MenuOption("3", "DEX", "Increases critical hit chance.");
        //MenuUI.MenuOption("4", "AGI", "Increases chances to dodge.");
        //MenuUI.ColoredMsg(ConsoleColor.Cyan, "══════════════════════════════════════════════════════════");
        //#endregion

        //Console.WriteLine("To go back enter [B]");
        //Console.Write($"Selection » ");
        //string? selection = Console.ReadLine().ToUpper();

        //if (selection == "B") return GameState.MainMenu;

        //Console.Write($"Amount to invest » ");
        //string? amountInput = Console.ReadLine();

        //if (int.TryParse(amountInput, out int amount))
        //{
        //    if (amount > 0 && amount <= p.UnusedStatPoints)
        //    {
        //        switch (selection)
        //        {
        //            case "1": p.InvestedSTRPoints += amount; break;
        //            case "2": p.InvestedVITPoints += amount; break;
        //            case "3": p.InvestedDEXPoints += amount; break;
        //            case "4": p.InvestedAGIPoints += amount; break;
        //            default:
        //                MenuUI.ColoredMsg(ConsoleColor.Yellow,"[SYSTEM] Invalid selection");
        //                Thread.Sleep(1000);
        //                return GameState.Training;
        //        }

        //        p.UnusedStatPoints -= amount;
        //        MenuUI.ColoredMsg(ConsoleColor.Yellow, $"[SYSTEM] Invested {amount} points");
        //        Thread.Sleep(1000);
        //    }
        //    else
        //    {
        //        MenuUI.ColoredMsg(ConsoleColor.Yellow, "[SYSTEM] Invalid amount or not enough points");
        //        Thread.Sleep(1000);
        //        return GameState.Training;
        //    }
        //}

        return GameState.MainMenu;
    }
    static void WriteStat(string label, int value)
    {
        //Console.ForegroundColor = ConsoleColor.Cyan;
        //Console.Write($"{label}:");
        //Console.ForegroundColor = ConsoleColor.White;
        //Console.Write($"{value,-3}");
        //Console.ForegroundColor = ConsoleColor.DarkGray;
        //Console.Write("  │ ");
    }
}