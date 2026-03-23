namespace TextBasedRPG.Core.Heroes
{
    internal class Warrior : Hero
    {
        public Warrior(HeroData data)
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
