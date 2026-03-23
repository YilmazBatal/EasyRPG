using System.Collections.Generic;
using TextBasedRPG.Core.Entities;
using TextBasedRPG.Models;

namespace TextBasedRPG.Core.Locations
{
    public class Location
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int LevelCap { get; set; }
        public List<string>? AdventureTexts { get; set; }
        public List<Loots>? AdventureLoots { get; set; }
        public List<string>? Entities { get; set; }
        public List<Entity> ActiveEntities { get; protected set; } = new();

        public Location(string id, string name, string? description, List<string>? texts, List<string>? entities, List<Loots> loots, int levelCap)
        {
            ID = id;
            Name = name;
            Description = description;
            LevelCap = levelCap;
            AdventureTexts = texts ?? new List<string>();
            AdventureLoots = loots ?? new List<Loots>();
            Entities = entities ?? new List<string>();
        }
    }
}
