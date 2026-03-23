
namespace TextBasedRPG.States
{
    public class InventoryState : IMenuState
    {
        public GameState Update(GameContext context)
        {
            //Console.Clear();
            //InventoryUI.BackpackPagination(context);
            return GameState.MainMenu;
        }

    }
}
