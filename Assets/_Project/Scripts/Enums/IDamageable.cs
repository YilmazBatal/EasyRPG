namespace TextBasedRPG.Interfaces
{
    internal interface IDamageable
    {
        int CurHP { get; }
        int TotalHP { get; }
        int TotalDEF { get; }

        void TakeDamage(int amount);
    }
}
