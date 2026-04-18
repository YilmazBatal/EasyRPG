using TextBasedRPG.Core.Items;
using TextBasedRPG.Managers.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets._Project.Scripts.UI.Cards;
using Material = TextBasedRPG.Core.Items.Material;

public class BackpackManager : MonoBehaviour
{
    #region Variables
    [Header("Inventory & Item Card Prefab")]
    [SerializeField] TMP_Text inventoryCapacityText;
    [SerializeField] GameObject inventoryContent;
    [SerializeField] GameObject itemCardPrefab;
    [SerializeField] GameObject detailsCardPrefab;
    [SerializeField] InventoryFilter inventoryFilter;
    
    GameContext context;
    #endregion

    private void OnEnable()
    {
        context = GameManager.Instance.Context;
        if (inventoryFilter != null)
        {
            inventoryFilter.onFilterChanged.AddListener(GenerateItemCards);
        }
        GenerateItemCards();
        inventoryCapacityText.text = $"{context.Player.Inventory.Count}/20"; // hard coded 20 for now
    }

    private void OnDisable()
    {
        if (inventoryFilter != null)
        {
            inventoryFilter.onFilterChanged.RemoveListener(GenerateItemCards);
        }
    }

    public void GenerateItemCards()
    {
        ClearItems();

        Weapon equippedWeapon = context.Player.EquippedWeapon;
        Armor equippedArmor = context.Player.EquippedArmor;

        // Only show equipped items if there is no filter or the filter allows them
        if (equippedWeapon != null && (inventoryFilter == null || inventoryFilter.ShouldShowItem(equippedWeapon)))
            ModifyItemCard(equippedWeapon, true);

        if (equippedArmor != null && (inventoryFilter == null || inventoryFilter.ShouldShowItem(equippedArmor)))
            ModifyItemCard(equippedArmor, true);

        foreach (var item in context.Player.Inventory)
        {
            Item masterItem = GetItemWithID(item.ID); // Get base item
            if (masterItem != null)
            {
                if (inventoryFilter != null && !inventoryFilter.ShouldShowItem(masterItem)) continue;

                Item newItem = masterItem.Clone();
                newItem.Quantity = item.Quantity;
                if (newItem is Weapon w) w.Upgrade = item.Upgrade;
                if (newItem is Armor a) a.Upgrade = item.Upgrade;

                ModifyItemCard(newItem); // Modify the item card with the item data and append to inventory content
            }
        }
    }

    private void ModifyItemCard(Item item, bool isEquipped = false)
    {
        GameObject newCard = Instantiate(itemCardPrefab, inventoryContent.transform);
        InventoryCard card = newCard.GetComponent<InventoryCard>();

        // Common properties
        card.itemName.text = item.Name;
        card.itemRarity.effectColor = UIManager.Instance.rarityColors[item.Rarity.ToString()];
        card.price.text = $"{item.Price}G";
        
        if (card.detailsBTN != null)
        {
            card.detailsBTN.interactable = true;
            card.detailsBTN.onClick.RemoveAllListeners();
            card.detailsBTN.onClick.AddListener(() => ShowDetails(item));
        }
        else
        {
            Debug.LogWarning("Details button is not assigned in the InventoryCard prefab!");
        }
        
        if (card.actionBTN != null) card.actionBTN.gameObject.SetActive(true); // ensure it is active by default
        
        // Specific properties
        if (item is Weapon w)
        {
            card.itemIcon.sprite = w.WeaponType switch
            {
                WeaponType.Sword => card.swordIcon,
                WeaponType.Bow => card.bowIcon,
                WeaponType.Staff => card.staffIcon,
                _ => card.swordIcon
            };

            card.subStatIcon1.sprite = card.attackIcon;
            card.subStat1.text = w.WeaponATK.ToString();

            card.subStatIcon2.sprite = card.levelIcon;
            card.subStat2.text = w.RequiredLevel.ToString();

            card.subStatIcon3.sprite = card.upgradeIcon;
            card.subStat3.text = $"+{w.Upgrade}";

            //card.detailsBTN
            string text = isEquipped ? "Unequip" : "Equip";
            card.actionBTNText.text = text;

            if (w.RequiredLevel >= context.Player.Level) // or not enough str etc for the future implementations
            {
                card.actionBTN.interactable = false;
            }
        }
        else if (item is Armor a)
        {
            card.itemIcon.sprite = card.armorIcon;

            card.subStatIcon1.sprite = card.armorIcon;
            card.subStat1.text = a.ArmorDef.ToString();

            card.subStatIcon2.sprite = card.levelIcon;
            card.subStat2.text = a.RequiredLevel.ToString();

            card.subStatIcon3.sprite = card.upgradeIcon;
            card.subStat3.text = $"+{a.Upgrade}";

            //card.detailsBTN
            string text = isEquipped ? "Unequip" : "Equip";
            card.actionBTNText.text = text;

            if (a.RequiredLevel >= context.Player.Level) // or not enough str etc for the future implementations
            {
                card.actionBTN.interactable = false;
            }
        }
        else if (item is Material m)
        {
            card.itemIcon.sprite = card.materialIcon;

            card.subStatIcon1.sprite = card.quantityIcon;
            card.subStat1.text = $"x{m.Quantity}";

            card.subStatIcon2.gameObject.SetActive(false);
            card.subStat2.gameObject.SetActive(false);
            
            card.subStatIcon3.gameObject.SetActive(false);
            card.subStat3.gameObject.SetActive(false);

            if (card.actionBTN != null) card.actionBTN.gameObject.SetActive(false);
            
            // Make details button full width with 24px padding and set its text to "Details"
            if (card.detailsBTN != null)
            {
                RectTransform dRt = card.detailsBTN.GetComponent<RectTransform>();
                if (dRt != null)
                {
                    dRt.anchorMin = new Vector2(0f, dRt.anchorMin.y);
                    dRt.anchorMax = new Vector2(1f, dRt.anchorMax.y);
                    dRt.offsetMin = new Vector2(24f, dRt.offsetMin.y);
                    dRt.offsetMax = new Vector2(-24f, dRt.offsetMax.y);
                }

                TMP_Text detailsText = card.detailsBTN.GetComponentInChildren<TMP_Text>();
                if (detailsText != null) detailsText.text = "Details";
            }
        }
        else if (item is Consumable c)
        {
            card.itemIcon.sprite = card.meatIcon;

            card.subStatIcon1.sprite = card.meatIcon;
            card.subStat1.text = c.Effect;

            card.subStatIcon2.sprite = card.hpIcon;
            card.subStat2.text = c.Value.ToString() + "%";

            card.subStatIcon3.gameObject.SetActive(false);
            card.subStat3.gameObject.SetActive(false);

            //card.detailsBTN
            card.actionBTNText.text = "Use";

        }
    }

    private Item GetItemWithID(string ID)
    {
        if (context.MasterItemBook.TryGetValue(ID, out var itemData))
        {
            return itemData;
        }
        else
        {
            Debug.LogWarning($"Item with ID {ID} not found in MasterItemBook.");
            return null;
        }
    }
    private void ClearItems()
    {
        foreach (Transform child in inventoryContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ShowDetails(Item item)
    {
        if (detailsCardPrefab == null) 
        {
            Debug.LogError("detailsCardPrefab is missing! Please assign it in the inspector.");
            return;
        }
        
        Canvas parentCanvas = GetComponentInParent<Canvas>();
        Transform parentTransform = parentCanvas != null ? parentCanvas.transform : transform;
        
        GameObject detailsObj = Instantiate(detailsCardPrefab, parentTransform);
        DetailsCard dc = detailsObj.GetComponent<DetailsCard>();
        if (dc == null) return;
        
        if (dc.title != null) dc.title.text = item.Name;
        if (dc.desc != null) dc.desc.text = item.Description;
        if (dc.gold != null) dc.gold.text = $"{item.Price}G";
        
        if (dc.icon1 != null) dc.icon1.gameObject.SetActive(false);
        if (dc.value1 != null) dc.value1.gameObject.SetActive(false);
        if (dc.icon2 != null) dc.icon2.gameObject.SetActive(false);
        if (dc.value2 != null) dc.value2.gameObject.SetActive(false);
        if (dc.icon3 != null) dc.icon3.gameObject.SetActive(false);
        if (dc.value3 != null) dc.value3.gameObject.SetActive(false);
        if (dc.icon4 != null) dc.icon4.gameObject.SetActive(false);
        if (dc.value4 != null) dc.value4.gameObject.SetActive(false);
        if (dc.icon5 != null) dc.icon5.gameObject.SetActive(false);
        if (dc.value5 != null) dc.value5.gameObject.SetActive(false);

        if (item is Weapon w)
        {
            SetDetailStat(dc, 1, "ATK", w.WeaponATK.ToString());
            SetDetailStat(dc, 2, "LVL", w.RequiredLevel.ToString());
            SetDetailStat(dc, 3, "UPG", $"+{w.Upgrade}");
        }
        else if (item is Armor a)
        {
            SetDetailStat(dc, 1, "DEF", a.ArmorDef.ToString());
            SetDetailStat(dc, 2, "HP", $"+{a.ExtraHP}");
            SetDetailStat(dc, 3, "LVL", a.RequiredLevel.ToString());
            SetDetailStat(dc, 4, "UPG", $"+{a.Upgrade}");
        }
        else if (item is Material m)
        {
            SetDetailStat(dc, 1, "QTY", $"x{m.Quantity}");
        }
        else if (item is Consumable c)
        {
            SetDetailStat(dc, 1, "EFF", c.Effect);
            SetDetailStat(dc, 2, "VAL", $"{c.Value}%");
        }
        
        if (dc.action != null)
        {
             dc.action.onClick.RemoveAllListeners();
             dc.action.onClick.AddListener(() => Destroy(detailsObj));
             TMP_Text btnText = dc.action.GetComponentInChildren<TMP_Text>();
             if (btnText != null) btnText.text = "Close";
        }
    }

    private void SetDetailStat(DetailsCard dc, int index, string iconText, string valueText)
    {
        TMP_Text icon = null;
        TMP_Text val = null;
        switch (index)
        {
            case 1: icon = dc.icon1; val = dc.value1; break;
            case 2: icon = dc.icon2; val = dc.value2; break;
            case 3: icon = dc.icon3; val = dc.value3; break;
            case 4: icon = dc.icon4; val = dc.value4; break;
            case 5: icon = dc.icon5; val = dc.value5; break;
        }
        
        if (icon != null) { icon.gameObject.SetActive(true); icon.text = iconText; }
        if (val != null) { val.gameObject.SetActive(true); val.text = valueText; }
    }
}
