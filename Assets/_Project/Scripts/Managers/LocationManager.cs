using System.Collections.Generic;
using System.Linq;

namespace TextBasedRPG.Managers
{
    internal static class LocationManager
    {
        public static Dictionary<string, string> locations = new ();
        public static int GetLocationIndex(GameContext context)
        {
            return locations.Keys.ToList().IndexOf(context.Player.ActiveLocation);
        }
        public static void LocationMapping(GameContext context)
        {
            foreach (var item in context.Locations)
            {
                locations.Add(item.ID, item.Name);
            }
        }
    }
}
