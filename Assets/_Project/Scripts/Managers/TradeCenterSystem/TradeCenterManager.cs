using System.Collections.Generic;
using System.Linq;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Core.Shops;
using TextBasedRPG.Managers.TradeCenterSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Trade Center panelini yöneten ana Manager.
///
/// UI Yapısı:
///   - TabGroup > BuyGroup / SellGroup  → sadece TAB BUTONLARI (görsel işaretleyici), içerik konteyneri değil!
///   - TabContent > ScrollView > Viewport > Content  → TEK bir içerik alanı, hem Buy hem Sell buraya yüklenir
///   - CategoryBG > Category (0/1/2)    → kategori butonları
///
/// Sorumluluklar:
///   - Mod geçişi (Buy/Sell) — tek Content temizlenip yeniden doldurulur
///   - Kategori seçimi (Weapon/Armor/Consumable/Material)
///   - Bölgeye göre Shop eşyalarını yükleme (Buy)
///   - Oyuncu envanterini listeleme (Sell, %50 fiyat)
///   - Satın alma ve satma işlemleri
/// </summary>
public class TradeCenterManager : MonoBehaviour
{
    // ─── Inspector Referansları ───────────────────────────────────────────

    [Header("Tab Visual Objects (Görsel işaretleyici - SetActive toggle için değil!)")]
    [Tooltip("TabGroup > BuyGroup — sadece Buy sekmesinin aktif görselini yönetmek için")]
    [SerializeField] private GameObject buyTabVisual;
    [Tooltip("TabGroup > SellGroup — sadece Sell sekmesinin aktif görselini yönetmek için")]
    [SerializeField] private GameObject sellTabVisual;

    [Header("Tab Buttons (Buy / Sell)")]
    [SerializeField] private Button buyTabButton;
    [SerializeField] private Button sellTabButton;

    [Header("Category Buttons")]
    [SerializeField] private Button weaponCategoryBtn;
    [SerializeField] private Button armorCategoryBtn;
    [SerializeField] private Button consumableCategoryBtn;
    [SerializeField] private Button materialCategoryBtn;

    [Header("Scroll View — TEK içerik alanı")]
    [Tooltip("ScrollView (1) > Viewport > Content objesini sürükle")]
    [SerializeField] private Transform contentParent;

    [Header("Prefab — Project klasöründeki gerçek prefab olmalı, sahnedeki değil!")]
    [Tooltip("Assets > Prefabs klasöründeki MarketItem prefabını buraya sürükle")]
    [SerializeField] private GameObject marketItemPrefab;

    [Header("Shop Info (Opsiyonel)")]
    [SerializeField] private TextMeshProUGUI shopNameText;
    [SerializeField] private TextMeshProUGUI playerGoldText;

    // ─── Durum Değişkenleri ───────────────────────────────────────────────

    private TradeMode _currentMode = TradeMode.Buy;
    private ItemType _currentCategory = ItemType.Weapon;
    private GameContext _context;
    private Shop _currentShop; // Aktif bölgenin dükkânı

    // ─── Unity Lifecycle ──────────────────────────────────────────────────

    private void Start()
    {
        _context = GameManager.Instance.Context;

        if (_context == null)
        {
            Debug.LogError("[TradeCenterManager] GameManager.Instance.Context null! StaticData yüklendi mi?");
            return;
        }

        // Dükkânı bölgeye göre bul
        LoadShopForCurrentLocation();

        // Tab butonlarını bağla
        buyTabButton?.onClick.AddListener(() => SwitchMode(TradeMode.Buy));
        sellTabButton?.onClick.AddListener(() => SwitchMode(TradeMode.Sell));

        // Kategori butonlarını bağla
        weaponCategoryBtn?.onClick.AddListener(() => SwitchCategory(ItemType.Weapon));
        armorCategoryBtn?.onClick.AddListener(() => SwitchCategory(ItemType.Armor));
        consumableCategoryBtn?.onClick.AddListener(() => SwitchCategory(ItemType.Consumable));
        materialCategoryBtn?.onClick.AddListener(() => SwitchCategory(ItemType.Material));

        // Başlangıç: Buy modu, Weapon kategorisi
        SwitchMode(TradeMode.Buy);
    }

    // ─── Panel / Kategori Geçiş ───────────────────────────────────────────

    /// <summary>
    /// Buy veya Sell moduna geçiş yapar.
    /// NOT: BuyGroup/SellGroup içerik konteyneri DEĞİL — sadece görsel tab işaretleyicileri.
    /// İçerik her zaman aynı Content'e yüklenir, sadece veri kaynağı değişir.
    /// </summary>
    public void SwitchMode(TradeMode mode)
    {
        _currentMode = mode;

        // Görsel tab aktif/pasif işaretleme — bunlar İÇERİK konteyneri değil!
        // Kendi UI yapına göre burada renk/scale animasyonu da yapabilirsin.
        // Eğer buyTabVisual/sellTabVisual yoksa bu satırlar null-safe çalışır.
        if (buyTabVisual != null)   buyTabVisual.SetActive(mode == TradeMode.Buy);
        if (sellTabVisual != null)  sellTabVisual.SetActive(mode == TradeMode.Sell);

        RefreshGoldUI();
        RefreshItems(); // Tek Content'i temizle ve yeniden doldur
    }

    /// <summary>Sol kategorilerden birini seçer ve içeriği yeniler.</summary>
    public void SwitchCategory(ItemType category)
    {
        _currentCategory = category;
        RefreshItems();
    }

    // ─── Veri Yükleme ────────────────────────────────────────────────────

    /// <summary>Oyuncunun aktif bölgesine göre Shop'u bulur.</summary>
    private void LoadShopForCurrentLocation()
    {
        if (_context.Shops == null || _context.Player == null)
        {
            Debug.LogWarning("[TradeCenterManager] Shops listesi veya Player null!");
            return;
        }

        string activeLocationID = _context.Player.ActiveLocation ?? "L001";
        Debug.Log($"[TradeCenterManager] Aktif bölge: {activeLocationID}");

        _currentShop = _context.Shops.FirstOrDefault(s => s.LocationID == activeLocationID);

        if (_currentShop == null)
        {
            Debug.LogWarning($"[TradeCenterManager] '{activeLocationID}' için dükkan bulunamadı. Shops.json'da bu LocationID var mı?");
        }
        else
        {
            Debug.Log($"[TradeCenterManager] Dükkan yüklendi: {_currentShop.ShopName} | Item sayısı: {_currentShop.Items?.Count ?? 0}");
            if (shopNameText != null)
                shopNameText.text = _currentShop.ShopName;
        }
    }

    /// <summary>Mevcut mod ve kategoriye göre tek Content'i yeniler.</summary>
    private void RefreshItems()
    {
        ClearContent();

        List<Item> items = _currentMode == TradeMode.Buy
            ? GetBuyItems()
            : GetSellItems();

        Debug.Log($"[TradeCenterManager] {_currentMode} | {_currentCategory} → {items.Count} eşya listeleniyor.");
        SpawnItemCards(items);
    }

    /// <summary>Buy: Bölgenin dükkânındaki eşyaları kategoriye göre filtreler.</summary>
    private List<Item> GetBuyItems()
    {
        if (_currentShop == null)
        {
            Debug.LogWarning("[TradeCenterManager] _currentShop null — Buy listesi boş.");
            return new List<Item>();
        }

        if (_context.MasterItemBook == null || _context.MasterItemBook.Count == 0)
        {
            Debug.LogWarning("[TradeCenterManager] MasterItemBook boş — StaticData.LoadStaticDatas çağrıldı mı?");
            return new List<Item>();
        }

        var result = _currentShop.Items
            .Where(id => !string.IsNullOrEmpty(id) && _context.MasterItemBook.ContainsKey(id))
            .Select(id => _context.MasterItemBook[id])
            .Where(item => item.ItemType.HasValue && item.ItemType.Value == _currentCategory)
            .ToList();

        return result;
    }

    /// <summary>Sell: Oyuncunun envanterindeki eşyaları kategoriye göre filtreler.</summary>
    private List<Item> GetSellItems()
    {
        if (_context.Player?.Inventory == null)
        {
            Debug.LogWarning("[TradeCenterManager] Player.Inventory null.");
            return new List<Item>();
        }

        if (_context.MasterItemBook == null || _context.MasterItemBook.Count == 0)
        {
            Debug.LogWarning("[TradeCenterManager] MasterItemBook boş.");
            return new List<Item>();
        }

        var result = _context.Player.Inventory
            .Where(inv => !string.IsNullOrEmpty(inv.ID) && _context.MasterItemBook.ContainsKey(inv.ID))
            .Select(inv => _context.MasterItemBook[inv.ID])
            .Where(item => item.ItemType.HasValue && item.ItemType.Value == _currentCategory)
            .ToList();

        return result;
    }

    // ─── UI Kartları ──────────────────────────────────────────────────────

    /// <summary>
    /// Eşya listesini TEK Content'e spawn eder.
    /// marketItemPrefab: Assets > Prefabs klasöründeki prefab olmalı (sahnedeki değil!).
    /// Sahnedeki MarketItem'lar tasarım örneği olarak duruyorsa play modunda silinmelerini
    /// önlemek için onları Content'in altından çıkar veya ayrı bir "template" objesine taşı.
    /// </summary>
    private void SpawnItemCards(List<Item> items)
    {
        if (marketItemPrefab == null)
        {
            Debug.LogError("[TradeCenterManager] marketItemPrefab atanmamış! " +
                           "Project klasöründeki prefabı Inspector'a sürükle, sahnedeki objeyi değil.");
            return;
        }

        if (contentParent == null)
        {
            Debug.LogError("[TradeCenterManager] contentParent atanmamış! ScrollView > Viewport > Content'i sürükle.");
            return;
        }

        foreach (Item item in items)
        {
            GameObject cardObj = Instantiate(marketItemPrefab, contentParent);
            MarketItemCard card = cardObj.GetComponent<MarketItemCard>();

            if (card != null)
                card.Setup(item, _currentMode, this);
            else
                Debug.LogWarning("[TradeCenterManager] MarketItem prefabında MarketItemCard bileşeni yok!");
        }
    }

    /// <summary>
    /// Content altındaki tüm dinamik kartları siler.
    /// ÖNEMLİ: marketItemPrefab sahnedeki Content altında bir objeye işaret ediyorsa
    /// o da silinir ve bir sonraki SpawnItemCards çöker. Prefab mutlaka Project'ten seçilmeli.
    /// </summary>
    private void ClearContent()
    {
        if (contentParent == null) return;

        // Immediate destroy yerine DestroyImmediate sorun çıkarmaz,
        // ama normal Destroy yeterli — çerçeve sonunda temizlenir.
        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }
    }

    // ─── İşlem Metodları (MarketItemCard çağırır) ─────────────────────────

    /// <summary>Bir eşya satın alındığında MarketItemCard.OnActionButtonClicked tarafından çağrılır.</summary>
    public void OnBuyItem(Item item)
    {
        if (_context?.Player == null) return;

        if (_context.Player.Gold < item.Price)
        {
            Debug.Log($"[TradeCenterManager] Yeterli altın yok! Gereken: {item.Price} G, Mevcut: {_context.Player.Gold} G");
            // TODO: UI feedback — yetersiz altın popup/mesajı
            return;
        }

        _context.Player.Gold -= item.Price;

        var existing = _context.Player.Inventory.FirstOrDefault(x => x.ID == item.ID);
        if (existing != null)
        {
            existing.Quantity++;
        }
        else
        {
            _context.Player.Inventory.Add(new TextBasedRPG.Models.InventoryData
            {
                InstanceID = System.Guid.NewGuid().ToString(),
                ID         = item.ID,
                Quantity   = 1,
                Upgrade    = 0
            });
        }

        Debug.Log($"[TradeCenterManager] Satın alındı: {item.Name} — {item.Price} G");
        RefreshGoldUI();
    }

    /// <summary>Bir eşya satıldığında MarketItemCard.OnActionButtonClicked tarafından çağrılır. Fiyat %50.</summary>
    public void OnSellItem(Item item)
    {
        if (_context?.Player == null) return;

        var inventoryEntry = _context.Player.Inventory.FirstOrDefault(x => x.ID == item.ID);

        if (inventoryEntry == null)
        {
            Debug.LogWarning($"[TradeCenterManager] Satılacak eşya envanterde bulunamadı: {item.ID}");
            return;
        }

        int sellPrice = Mathf.FloorToInt(item.Price * 0.5f);
        _context.Player.Gold += sellPrice;

        if (inventoryEntry.Quantity > 1)
            inventoryEntry.Quantity--;
        else
            _context.Player.Inventory.Remove(inventoryEntry);

        Debug.Log($"[TradeCenterManager] Satıldı: {item.Name} — {sellPrice} G (%50)");
        RefreshGoldUI();
        RefreshItems();
    }

    // ─── UI Yardımcıları ──────────────────────────────────────────────────

    private void RefreshGoldUI()
    {
        if (playerGoldText != null && _context?.Player != null)
            playerGoldText.text = $"{_context.Player.Gold} G";
    }
}
