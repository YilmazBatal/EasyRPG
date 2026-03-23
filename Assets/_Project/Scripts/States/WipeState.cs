namespace TextBasedRPG.States
{
    internal class WipeState : IMenuState
    {
        public GameState Update(GameContext context)
        {
            //Console.Clear();
            //MenuUI.ColoredMsg(ConsoleColor.DarkRed, "ARE YOU SURE U WANT TO DELETE YOUR DATA?");
            //Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write("Type ");
            //Console.ForegroundColor = ConsoleColor.Red; Console.Write("'DELETE'");
            //Console.ForegroundColor = ConsoleColor.DarkYellow; Console.Write("to delete the data.\n");
            //Console.ResetColor();

            //string input = Console.ReadLine();

            //if (input == "DELETE" && File.Exists("data.json"))
            //{
            //    File.Delete("data.json");
            //    context.Player = null;
            //    Console.WriteLine("[SYSTEM] File has been deleted");
            //    return GameState.HeroSelection;
            //}
            //else
            //{
            //    return GameState.MainMenu;
            //}
            return GameState.MainMenu;

        }
    }
}
