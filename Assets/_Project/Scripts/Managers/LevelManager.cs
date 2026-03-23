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
                //Console.WriteLine($"\n[SYSTEM] Congratulations! You're now level {p.Level}!");
            }
        }
    }
}
