using System.Collections.Generic;

namespace TextBasedRPG.Models
{
    public class ShopData
    {
        public string ID { get; set; }
        public string ShopName { get; set; }
        public List<string> Items { get; set; }
    }
}
