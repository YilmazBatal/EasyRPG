public class DungeonState : IMenuState
{
    public GameState Update(GameContext context)
    {
        //Console.Clear();
        //Console.WriteLine("You are at the Dungeon State... Press any key to continue.");
        //Console.ReadKey(true);
        return GameState.MainMenu;
    }
}