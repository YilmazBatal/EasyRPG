using TMPro;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Core.Shops;
using UnityEngine;
using UnityEngine.UI;
using Assets._Project.Scripts.Enums;

namespace TextBasedRPG.Managers.TradeCenterSystem
{
    /// <summary>
    /// MarketItem prefab'ına bağlanan bileşen.
    /// TradeCenterManager tarafından SpawnItemCards çağrılırken Setup ile doldurulur.
    /// </summary>
    public class MarketItemCard : MonoBehaviour
    {
        [Header("Text References")]
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI itemTypeText;
        [SerializeField] private TextMeshProUGUI itemRarityText;
        [SerializeField] private TextMeshProUGUI itemPriceText;
        [SerializeField] private TextMeshProUGUI reqLevelText;

        [Header("Optional")]
        [SerializeField] private Image rarityColorBar;  // Rarity rengini gösteren arka plan/bar (opsiyonel)

        // Satın alma veya satma butonuna tıklandığında Manager'ı haberdar etmek için
        private Item _item;
        private TradeMode _mode;
        private TradeCenterManager _manager;

        /// <summary>
        /// Her card spawn edildiğinde bu metod çağrılır.
        /// </summary>
        /// <param name="item">Gösterilecek eşya</param>
        /// <param name="mode">Buy mu Sell mı</param>
        /// <param name="manager">İşlem callback'i için manager referansı</param>
        public void Setup(Item item, TradeMode mode, TradeCenterManager manager)
        {
            _item = item;
            _mode = mode;
            _manager = manager;

            // İsim
            if (itemNameText != null)
                itemNameText.text = item.Name ?? "Unknown";

            // Tip (Weapon / Armor / Consumable vb.)
            if (itemTypeText != null)
                itemTypeText.text = item.ItemType.HasValue ? item.ItemType.Value.ToString() : "-";

            // Rarity
            if (itemRarityText != null)
                itemRarityText.text = item.Rarity.ToString();

            // Fiyat: Buy → normal fiyat, Sell → %50
            if (itemPriceText != null)
            {
                int displayPrice = mode == TradeMode.Sell
                    ? Mathf.FloorToInt(item.Price * 0.5f)
                    : item.Price;
                itemPriceText.text = $"{displayPrice} G";
            }

            // Required Level — sadece Weapon ve Armor'da var
            if (reqLevelText != null)
            {
                int reqLevel = GetRequiredLevel(item);
                reqLevelText.text = reqLevel > 0 ? $"Lv. {reqLevel}" : "-";
            }

            // Rarity rengi (opsiyonel)
            if (rarityColorBar != null)
                rarityColorBar.color = GetRarityColor(item.Rarity);
        }

        // Inspector'daki "Buy / Sell" butonuna bağlanacak
        public void OnActionButtonClicked()
        {
            if (_manager == null || _item == null) return;

            if (_mode == TradeMode.Buy)
                _manager.OnBuyItem(_item);
            else
                _manager.OnSellItem(_item);
        }

        // ─── Yardımcı Metodlar ───────────────────────────────────────────

        private static int GetRequiredLevel(Item item)
        {
            if (item is Weapon w) return w.RequiredLevel;
            if (item is Armor a) return a.RequiredLevel;
            return 0;
        }

        private static Color GetRarityColor(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Common    => new Color(0.75f, 0.75f, 0.75f), // gri
                Rarity.Uncommon  => new Color(0.30f, 0.80f, 0.30f), // yeşil
                Rarity.Rare      => new Color(0.20f, 0.50f, 1.00f), // mavi
                Rarity.Epic      => new Color(0.65f, 0.20f, 1.00f), // mor
                Rarity.Legendary => new Color(1.00f, 0.65f, 0.00f), // altın
                _                => Color.white
            };
        }
    }
}
