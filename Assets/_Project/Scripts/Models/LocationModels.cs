using System.Collections.Generic;

namespace TextBasedRPG.Models
{
    public class LocationData
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int LevelCap { get; set; }
        public List<string>? AdventureTexts { get; set; }
        public List<Loots>? AdventureLoots { get; set; }
        public List<string>? Entities { get; set; }

    }
    public class Loots
    {
        public string ID { get; set; }
        public int DropChance { get; set; }
        public int MaxAmount { get; set; }
    }
}
