using System.Collections.Generic;
using System.Linq;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Core.Shops;
using TextBasedRPG.Events;
using TextBasedRPG.Managers;
using TextBasedRPG.Managers.TradeCenterSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeCenterManager : MonoBehaviour
{
    #region Inspector References
    [Header("Tab Buttons (Buy / Sell)")]
    [SerializeField] private Button buyTabButton;
    [SerializeField] private Button sellTabButton;
    [SerializeField] private TextMeshProUGUI modeText;
    [Range(0f, 1f)] [SerializeField] private float inactiveTabAlpha = 0.35f;

    [Header("Category Objects")]
    [Tooltip("Category objects mapped in order: Weapon, Armor, Consumable, Material")]
    [SerializeField] private GameObject[] categoryObjects;

    [Header("Content Container")]
    [SerializeField] private Transform contentParent;

    [Header("Prefab")]
    [SerializeField] private GameObject marketItemPrefab;

    [Header("Shop Info")]
    [SerializeField] private TextMeshProUGUI shopNameText;
    #endregion

    private TradeMode _currentMode = TradeMode.Buy;
    private ItemType _currentCategory = ItemType.Weapon;
    private GameContext _context;
    private Shop _currentShop;

    private void Start()
    {
        _context = GameManager.Instance.Context;

        if (_context == null)
        {
            Debug.LogError("[TradeCenterManager] GameManager.Instance.Context is null!");
            return;
        }

        LoadShopForCurrentLocation();

        buyTabButton?.onClick.AddListener(() => SwitchMode(TradeMode.Buy));
        sellTabButton?.onClick.AddListener(() => SwitchMode(TradeMode.Sell));

        if (categoryObjects != null)
        {
            if (categoryObjects.Length > 0 && categoryObjects[0] != null)
                categoryObjects[0].GetComponent<Button>()?.onClick.AddListener(() => SwitchCategory(ItemType.Weapon));
            if (categoryObjects.Length > 1 && categoryObjects[1] != null)
                categoryObjects[1].GetComponent<Button>()?.onClick.AddListener(() => SwitchCategory(ItemType.Armor));
            if (categoryObjects.Length > 2 && categoryObjects[2] != null)
                categoryObjects[2].GetComponent<Button>()?.onClick.AddListener(() => SwitchCategory(ItemType.Consumable));
            if (categoryObjects.Length > 3 && categoryObjects[3] != null)
                categoryObjects[3].GetComponent<Button>()?.onClick.AddListener(() => SwitchCategory(ItemType.Material));
        }

        UpdateCategoryVisuals();
        SwitchMode(TradeMode.Buy);
    }

    public void SwitchMode(TradeMode mode)
    {
        _currentMode = mode;

        if (buyTabButton != null)
            SetImageAlpha(buyTabButton.GetComponent<Image>(), mode == TradeMode.Buy ? 1f : inactiveTabAlpha);

        if (sellTabButton != null)
            SetImageAlpha(sellTabButton.GetComponent<Image>(), mode == TradeMode.Sell ? 1f : inactiveTabAlpha);

        modeText.text = mode == TradeMode.Buy ? "BUYING" : "SELLING";
        
        if (mode == TradeMode.Buy)
            categoryObjects[3].SetActive(false);
        else
            categoryObjects[3].SetActive(true);

        RefreshItems();
    }

    public void SwitchCategory(ItemType category)
    {
        _currentCategory = category;
        UpdateCategoryVisuals();
        RefreshItems();
    }

    private void LoadShopForCurrentLocation()
    {
        if (_context.Shops == null || _context.Player == null)
        {
            Debug.LogWarning("[TradeCenterManager] Shops or Player context is null.");
            return;
        }

        string activeLocationID = _context.Player.ActiveLocation ?? "L001";
        _currentShop = _context.Shops.FirstOrDefault(s => s.LocationID == activeLocationID);

        if (_currentShop == null)
        {
            Debug.LogWarning($"[TradeCenterManager] Shop not found for location: {activeLocationID}");
        }
        else
        {
            if (shopNameText != null)
                shopNameText.text = _currentShop.ShopName;
        }
    }

    private void RefreshItems()
    {
        ClearContent();

        List<Item> items = _currentMode == TradeMode.Buy
            ? GetBuyItems()
            : GetSellItems();

        SpawnItemCards(items);
    }

    private List<Item> GetBuyItems()
    {
        if (_currentShop == null || _context.MasterItemBook == null)
            return new List<Item>();

        return _currentShop.Items
            .Where(id => !string.IsNullOrEmpty(id) && _context.MasterItemBook.ContainsKey(id))
            .Select(id => _context.MasterItemBook[id])
            .Where(item => item.ItemType.HasValue && item.ItemType.Value == _currentCategory)
            .ToList();
    }

    private List<Item> GetSellItems()
    {
        if (_context.Player?.Inventory == null || _context.MasterItemBook == null)
            return new List<Item>();

        var items = new List<Item>();

        foreach (var inv in _context.Player.Inventory)
        {
            if (inv == null || string.IsNullOrEmpty(inv.ID)) continue;

            if (_context.MasterItemBook.TryGetValue(inv.ID, out var masterItem))
            {
                if (masterItem.ItemType.HasValue && masterItem.ItemType.Value == _currentCategory)
                {
                    Item clone = masterItem.Clone();
                    clone.Quantity = inv.Quantity;
                    if (clone is Weapon w) w.Upgrade = inv.Upgrade;
                    if (clone is Armor a) a.Upgrade = inv.Upgrade;
                    items.Add(clone);
                }
            }
        }

        return items;
    }

    private void SpawnItemCards(List<Item> items)
    {
        if (marketItemPrefab == null || contentParent == null) return;

        foreach (Item item in items)
        {
            GameObject cardObj = Instantiate(marketItemPrefab, contentParent);
            MarketItemCard card = cardObj.GetComponent<MarketItemCard>();

            if (card != null)
                card.Setup(item, _currentMode, this);
        }
    }

    private void ClearContent()
    {
        if (contentParent == null) return;

        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }
    }

    public void OnBuyItem(Item item)
    {
        if (_context?.Player == null || item == null) return;

        if (_context.Player.Gold < item.Price)
        {
            Toaster.Instance.ShowToast($"Not enough gold! ({item.Price}G)", UIManager.Instance.IconDB.lockedIcon);
            return;
        }

        _context.Player.Gold -= item.Price;
        EventManager.HeroEvents.TriggerGoldChanged(_context);

        InventoryManager.AddToInventory(item.ID, 1);

        Toaster.Instance.ShowToast($"{item.Name} purchased for {item.Price}G.", UIManager.Instance.IconDB.confirmIcon);

        if (_currentMode == TradeMode.Sell)
            RefreshItems();
    }

    public void OnSellItem(Item item)
    {
        if (_context?.Player == null || item == null) return;

        int sellPrice = Mathf.FloorToInt(item.Price * 0.5f);

        bool removed = InventoryManager.RemoveFromInventory(item.ID, 1);
        if (!removed)
        {
            Toaster.Instance.ShowToast($"Item not found in inventory: {item.ID}", UIManager.Instance.IconDB.lockedIcon);
            return;
        }

        _context.Player.Gold += sellPrice;
        EventManager.HeroEvents.TriggerGoldChanged(_context);

        Toaster.Instance.ShowToast($"{item.Name} sold for {sellPrice}G.", UIManager.Instance.IconDB.confirmIcon);
        RefreshItems();
    }

    private void UpdateCategoryVisuals()
    {
        if (categoryObjects == null) return;

        ItemType[] categoryTypes = { ItemType.Weapon, ItemType.Armor, ItemType.Consumable, ItemType.Material };

        for (int i = 0; i < categoryObjects.Length && i < categoryTypes.Length; i++)
        {
            GameObject catObj = categoryObjects[i];
            if (catObj == null) continue;

            bool isActive = categoryTypes[i] == _currentCategory;

            Image rootImage = catObj.GetComponent<Image>();
            if (rootImage != null)
                rootImage.enabled = isActive;

            Transform childImage = catObj.transform.Find("Image");
            if (childImage != null)
                childImage.gameObject.SetActive(isActive);
        }
    }

    private static void SetImageAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}

