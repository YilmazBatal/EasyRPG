namespace TextBasedRPG.Core.Heroes
{
    internal class Archer : Hero
    {
        public Archer(HeroData data)
        {
            ClassName = data.className;
            Description = data.description;
            BaseHP = data.hp;
            BaseATK = data.atk;
            BaseDEF = data.def;
            BaseSPD = data.spd;
            CurHP = TotalHP;
        }
    }
}
