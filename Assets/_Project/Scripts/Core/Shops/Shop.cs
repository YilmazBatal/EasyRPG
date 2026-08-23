using System.Collections.Generic;

namespace TextBasedRPG.Core.Shops
{
    public class Shop
    {
        public string ID { get; set; }
        public string ShopName { get; set; }
        public string LocationID { get; set; }
        public List<string> Items { get; set; }

        public Shop(string id, string shopName, string locationID, List<string> items)
        {
            ID = id;
            ShopName = shopName;
            LocationID = locationID;
            Items = items;
        }
    }
}
