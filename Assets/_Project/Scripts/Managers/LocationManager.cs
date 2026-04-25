using System.Collections.Generic;
using System.Linq;
using TextBasedRPG.Core.Locations;

namespace TextBasedRPG.Managers
{
    internal static class LocationManager
    {
        public static Dictionary<string, string> locations = new (); //id name
        public static string GetLocationName(string locationID)
        {
            return locations.ContainsKey(locationID) ? locations[locationID] : "Unknown Location";
        }
        public static int GetLocationIndex(GameContext context)
        {
            return locations.Keys.ToList().IndexOf(context.Player.ActiveLocation);
        }
        public static Location GetLocationByID(string locationID)
        {
            return GameManager.Instance.Context.Locations.FirstOrDefault(loc => loc.ID == locationID);
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
