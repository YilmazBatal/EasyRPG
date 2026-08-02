using Assets._Project.Scripts.Enums;

namespace TextBasedRPG.Core.Items
{
    public abstract class Item
    {
        public string ID { get; set; }
        public ItemType? ItemType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public Rarity Rarity { get; set; }
        public int Quantity { get; set; }

        protected Item(string id, ItemType itemType, string name, string? description, int price, Rarity rarity, int quantity)
        {
            ID = id;
            ItemType = itemType;
            Name = name;
            Description = description;
            Price = price;
            Rarity = rarity;
            Quantity = quantity;
        }
        public virtual Item Clone()
        {
            return (Item)this.MemberwiseClone();
        }
    }
}
