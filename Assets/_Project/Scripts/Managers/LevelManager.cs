namespace TextBasedRPG.Managers
{
    public static class LevelManager
    {
        public static void CheckLevelUp(GameContext context)
        {
            var p = context.Player!;
            while (p.CurExp >= p.ReqExp)
            {
                p.CurExp -= p.ReqExp;
                p.Level++;
                p.UnusedStatPoints += 4;
                Toaster.Instance.ShowToast($"GG! You are one step closer to achieving your villain dreams! You are now level {p.Level}!", UIManager.Instance.IconDB.confirmIcon);
                GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);    
            }
        }
    }
}
