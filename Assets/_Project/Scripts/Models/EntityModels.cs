using System.Collections.Generic;

namespace TextBasedRPG.Models
{
    public class MobData
    {
        public string ID { get; set; }
        public string EntityTypeID { get; set; }
        public string Name { get; set; }
        public int BaseHP { get; set; }
        public int BaseATK { get; set; }
        public int BaseDEF { get; set; }
        public int BaseSPD { get; set; }
        public string EntitySprite { get; set; }
        public int Level { get; set; }
        public int Scaling { get; set; }
        public int EliteChance { get; set; }
        public List<Loots> Loots { get; set; } = new();
        public List<string> Locations { get; set; } = new();
        public float GoldMultiplier { get; set; }
        public string EntityType = string.Empty;
    }
}
