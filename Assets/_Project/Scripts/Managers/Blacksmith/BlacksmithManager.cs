using System.Collections.Generic;
using TextBasedRPG.Core.Items;
using UnityEngine;
using UnityEngine.UI;

public class BlacksmithManager : MonoBehaviour
{
    #region Inspector References
    [Header("Filter Buttons (Weapon/Armor)")]
    [SerializeField] private Button weaponFilter;
    [SerializeField] private Button armorFilter;
    [Range(0f, 1f)][SerializeField] private float inactiveTabAlpha = 0.35f;
    [SerializeField] private Transform selectionsParent; // this is where the selection cards will be instantiated

    [Header("Prefab")]
    [SerializeField] private GameObject equipmentSelectionCard;
    #endregion

    private ItemType _currentCategory = ItemType.Weapon;
    private GameContext _context;

    private void OnEnable()
    {
        TextBasedRPG.Events.EventManager.HeroEvents.OnEquipmentChanged += OnEquipmentChanged;
    }

    private void OnDisable()
    {
        TextBasedRPG.Events.EventManager.HeroEvents.OnEquipmentChanged -= OnEquipmentChanged;
    }

    private void OnEquipmentChanged(GameContext ctx)
    {
        RefreshItems();
    }

    private void Start()
    {
        _context = GameManager.Instance.Context;

        if (_context == null)
        {
            Debug.LogError("[BlacksmithManager] GameManager.Instance.Context is null!");
            return;
        }

        weaponFilter?.onClick.AddListener(() => SwitchCategory(ItemType.Weapon));
        armorFilter?.onClick.AddListener(() => SwitchCategory(ItemType.Armor));

        Debug.Log ("[BlacksmithManager] Filters initialized!");


        SwitchCategory(ItemType.Weapon);
    }

    public void SwitchCategory(ItemType category)
    {
        _currentCategory = category;
        RefreshItems();
    }

    private static void SetImageAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    private void RefreshItems()
    {
        ClearContent();

        if (weaponFilter != null)
            SetImageAlpha(weaponFilter.GetComponent<Image>(), _currentCategory == ItemType.Weapon ? 1f : inactiveTabAlpha);
        
        if (armorFilter != null)
            SetImageAlpha(armorFilter.GetComponent<Image>(), _currentCategory == ItemType.Armor ? 1f : inactiveTabAlpha);

        // Add Equipped Items
        if (_currentCategory == ItemType.Weapon && _context.Player.EquippedWeapon != null)
        {
            CreateItemCard(_context.Player.EquippedWeapon);
        }
        else if (_currentCategory == ItemType.Armor && _context.Player.EquippedArmor != null)
        {
            CreateItemCard(_context.Player.EquippedArmor);
        }

        // Add Inventory Items
        if (_context.Player.Inventory != null)
        {
            foreach (var invItem in _context.Player.Inventory)
            {
                if (string.IsNullOrEmpty(invItem.ID)) continue;

                Item masterItem = GetItemWithID(invItem.ID);
                if (masterItem == null) continue;

                if (masterItem.ItemType != _currentCategory) continue;
                
                Item newItem = masterItem.Clone();
                if (newItem is Weapon w) w.Upgrade = invItem.Upgrade;
                if (newItem is Armor a) a.Upgrade = invItem.Upgrade;

                CreateItemCard(newItem);
            }
        }
    }

    private void CreateItemCard(Item item)
    {
        if (equipmentSelectionCard == null || selectionsParent == null) return;
        
        GameObject newCard = Instantiate(equipmentSelectionCard, selectionsParent);
        UpgradeSelectionCard card = newCard.GetComponent<UpgradeSelectionCard>();

        if (card != null)
        {
            var selectionUpgrade = FindObjectOfType<Assets._Project.Scripts.Managers.Blacksmith.SelectionUpgrade>();
            var iconDB = UIManager.Instance.IconDB;
            card.Setup(item, selectionUpgrade, iconDB);
        }
        else
        {
            // fallback in case the prefab still uses InventoryCard
            InventoryCard oldCard = newCard.GetComponent<InventoryCard>();
            if (oldCard != null)
            {
                oldCard.ModifyItemCard(item);
                if (oldCard.actionBTN != null)
                {
                    oldCard.actionBTN.onClick.RemoveAllListeners();
                    if (oldCard.actionBTNText != null) oldCard.actionBTNText.text = "Select";
                    oldCard.actionBTN.onClick.AddListener(() =>
                    {
                        var selectionUpgrade = FindObjectOfType<Assets._Project.Scripts.Managers.Blacksmith.SelectionUpgrade>();
                        if (selectionUpgrade != null) selectionUpgrade.Setup(item);
                    });
                }
                if (oldCard.discardBTN != null) oldCard.discardBTN.gameObject.SetActive(false);
            }
        }
    }

    private Item GetItemWithID(string ID)
    {
        if (_context != null && _context.MasterItemBook.TryGetValue(ID, out var itemData))
        {
            return itemData;
        }
        return null;
    }

    private void ClearContent()
    {
        if (selectionsParent == null) return;
        
        foreach (Transform child in selectionsParent)
        {
            Destroy(child.gameObject);
        }
    }
}
