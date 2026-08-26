using Assets._Project.Scripts.ScriptableObjects.ScriptableObjectScripts;
using TextBasedRPG.Core.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using Material = TextBasedRPG.Core.Items.Material;
using Item = TextBasedRPG.Core.Items.Item;

namespace TextBasedRPG.Managers.TradeCenterSystem
{
    [System.Serializable]
    public struct StatRow
    {
        public GameObject root;       // Stat yoksa satýrý komple gizlemek için
        public Image icon;
        public TMP_Text labelText;
        public TMP_Text valueText;

        public void SetStat(Sprite statIcon, string label, string value)
        {
            root.SetActive(true);
            icon.sprite = statIcon;
            labelText.text = label;
            valueText.text = value;
        }

        public void Hide() => root.SetActive(false);
    }
    public class MarketItemCard : MonoBehaviour
    {
        [Header("Text References")]
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private Outline itemFrameColor;
        [SerializeField] private TextMeshProUGUI itemRarityText;
        [SerializeField] private TextMeshProUGUI itemPriceText;

        [Header("Stats Rows (Size: 3)")]
        [SerializeField] private StatRow[] statRows;

        [Header("Button References")]
        [SerializeField] private Button actionButton;
        [SerializeField] private Button detailsButton;

        [Header("Rarity Styles")]
        [SerializeField] private RarityDatabase rarityDB;

        private Item _item;
        private TradeMode _mode;
        private TradeCenterManager _manager;
        private IconDatabase iconDB;

        public void Setup(Item item, TradeMode mode, TradeCenterManager manager)
        {
            iconDB = UIManager.Instance.IconDB;

            _item = item;
            _mode = mode;
            _manager = manager;

            itemNameText.enableVertexGradient = true;
            itemNameText.colorGradientPreset = rarityDB.GetGradient(item.Rarity);
            itemNameText.text = item.Name ?? "Unknown";

            itemFrameColor.effectColor = rarityDB.GetColor(item.Rarity);

            itemRarityText.text = item.Rarity.ToString();
            itemRarityText.color = rarityDB.GetColor(item.Rarity);

            if (itemPriceText != null)
            {
                int displayPrice = mode == TradeMode.Sell
                    ? Mathf.FloorToInt(item.Price * 0.5f)
                    : item.Price;
                itemPriceText.text = $"{displayPrice}G";
            }

            if (actionButton != null)
            {
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(OnActionButtonClicked);
            }

            if (detailsButton != null)
            {
                detailsButton.onClick.RemoveAllListeners();
                detailsButton.onClick.AddListener(OnDetailsButtonClicked);
            }

            if (_item is Weapon w)
            {
                itemIcon.sprite = iconDB.GetWeaponIcon(w.WeaponType);

                if (_mode == TradeMode.Sell)
                {
                    statRows[0].SetStat(iconDB.GetWeaponIcon(w.WeaponType), "ATK", $"{w.WeaponATK}");
                    statRows[1].SetStat(iconDB.levelIcon, "Level", $"{w.RequiredLevel}");
                    statRows[2].SetStat(iconDB.upgradeIcon, "Upgrade", $"+{w.Upgrade}");
                } else
                {
                    statRows[0].SetStat(iconDB.GetWeaponIcon(w.WeaponType), "ATK", $"{w.WeaponATK}");
                    statRows[1].SetStat(iconDB.levelIcon, "Level", $"{w.RequiredLevel}");
                    statRows[2].Hide();
                }
            }
            else if (_item is Armor a)
            {
                itemIcon.sprite = iconDB.armorIcon;

                if (_mode == TradeMode.Sell)
                {
                    statRows[0].SetStat(iconDB.armorIcon, "DEF", $"{a.ArmorDef}");
                    statRows[1].SetStat(iconDB.hpIcon, "Extra HP", $"{a.ExtraHP}");
                    statRows[2].SetStat(iconDB.upgradeIcon, "Upgrade", $"+{a.Upgrade}");
                }
                else
                {
                    statRows[0].SetStat(iconDB.armorIcon, "DEF", $"{a.ArmorDef}");
                    statRows[1].SetStat(iconDB.hpIcon, "Extra HP", $"{a.ExtraHP}");
                    statRows[2].SetStat(iconDB.levelIcon, "Level", $"{a.RequiredLevel}");
                }
                
            }
            else if (_item is Consumable c)
            {
                string combatItemText = c.CombatItem ? "Yes" : "No";
                itemIcon.sprite = iconDB.potIcon;

                statRows[0].SetStat(iconDB.meatIcon, "Effect:", $"{c.Effect}");
                statRows[1].SetStat(iconDB.plusIcon, "Value", $"{c.Value}");
                statRows[2].SetStat(iconDB.hammerIcon, "Combat Item", $"{combatItemText}");
            }
            else if (_item is Material m)
            {
                itemIcon.sprite = iconDB.boneIcon;

                if (_mode == TradeMode.Sell)
                {
                    statRows[0].SetStat(iconDB.quantityIcon, "Quantity:", $"{m.Quantity}");
                    statRows[1].Hide();
                    statRows[2].Hide();
                }
                else
                {
                    statRows[0].Hide();
                    statRows[1].Hide();
                    statRows[2].Hide();
                }
            }
        }
        public void OnActionButtonClicked()
        {
            if (_manager == null || _item == null) return;

            if (_mode == TradeMode.Buy)
                _manager.OnBuyItem(_item);
            else
                _manager.OnSellItem(_item);
        }

        public void OnDetailsButtonClicked()
        {
            if (_manager == null || _item == null) return;

            Toaster.Instance.ShowToast($"Details panel not implemented yet.", UIManager.Instance.IconDB.lockedIcon);

        }
    }
}

