namespace TextBasedRPG.Core.Heroes
{
    internal class Mage : Hero
    {
        public Mage(HeroData data)
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
