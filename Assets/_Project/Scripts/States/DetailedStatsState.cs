
namespace TextBasedRPG.States
{
    public class DetailedStatsState : IMenuState
    {
        public GameState Update(GameContext context)
        {
            //Console.Clear();
            //var p = context.Player;
            //Console.WriteLine("================ Detailed Stats for Player ================");
            //Console.WriteLine($"{"Class:",-25} {p?.ClassName}");
            //Console.WriteLine($"{"Level:",-25} {p?.Level,-5} [{p?.CurExp}/{p?.ReqExp}]");
            //Console.WriteLine($"{"Gold:",-25} {p?.Gold}");
            //Console.WriteLine("-----------------------------------------------------------");
            //Console.WriteLine($"{"Total ATK:",-25} {p?.TotalATK:F1}");
            //Console.WriteLine($"{"Total DEF:",-25} {p?.TotalDEF:F1}");
            //Console.WriteLine($"{"Total HP:",-25} {p?.TotalHP}");
            //Console.WriteLine($"{"Crit Rate:",-25} %{p?.CritRate:F1}");
            //Console.WriteLine($"{"Crit DMG:",-25} %{p?.CritDamage:F1}");
            //Console.WriteLine($"{"Evasion:",-25} %{p?.EvasionRate:F1}");
            //Console.WriteLine("------------------- Equipment Data ------------------------");
            //CharacterUI.EquipmentCheck(context);
            //Console.WriteLine("------------------- Training Data -------------------------");
            //Console.WriteLine($"Unused Training Points: {p?.UnusedStatPoints}");
            //Console.WriteLine($"Total Points Invested: {p?.InvestedSTRPoints + p?.InvestedVITPoints + p?.InvestedDEXPoints + p?.InvestedAGIPoints}");
            //Console.WriteLine($"{"STR:",-25} {p?.InvestedSTRPoints}");
            //Console.WriteLine($"{"VIT:",-25} {p?.InvestedVITPoints}");
            //Console.WriteLine($"{"DEX:",-25} {p?.InvestedDEXPoints}");
            //Console.WriteLine($"{"AGI:",-25} {p?.InvestedAGIPoints}");
            //Console.WriteLine("===========================================================");
            //Console.Write("[SYSTEM] Press any key to go back...");
            //Console.ReadKey(true);

            return GameState.MainMenu;
        }
    }
}
