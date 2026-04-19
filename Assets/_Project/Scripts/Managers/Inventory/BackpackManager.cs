using Assets._Project.Scripts.UI.Cards;
using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Managers.Inventory;
using TextBasedRPG.Models;
using TextBasedRPG.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
            card.detailsBTN.onClick.AddListener(() => ShowDetails(item, card));
        }

        if (card.actionBTN != null) card.actionBTN.gameObject.SetActive(true); // ensure it is active by default

        // Specific properties
        ModifyWeaponCard(item, isEquipped, card);
        ModifyArmorCard(item, isEquipped, card);
        ModifyMaterialCard(item, isEquipped, card);
        ModifyConsumableCard(item, isEquipped, card);
    }
    private void ShowDetails(Item item, InventoryCard itemCard)
    {
        if (detailsCardPrefab == null)
        {
            Debug.LogError("detailsCardPrefab is missing! Please assign it in the inspector.");
            return;
        }


        GameObject detailsObj = Instantiate(detailsCardPrefab, transform);

        OpenDetailsMenu(detailsObj);

        DetailsCard dc = detailsObj.transform.GetChild(0).GetComponent<DetailsCard>();
        if (dc == null) return;
        // common properties
        dc.title.text = item.Name;
        dc.desc.text = item.Description;
        dc.gold.text = $"{item.Price}G";
        dc.itemIcon.sprite = itemCard.itemIcon.sprite;
        dc.itemIcon.transform.parent.GetComponent<Outline>().effectColor = UIManager.Instance.rarityColors[item.Rarity.ToString()];

        ModifyWeaponDetailsCard(item, itemCard, dc);
        ModifyArmorDetailsCard(item, itemCard, dc);
        ModifyMaterialDetailsCard(item, itemCard, dc);
        ModifyConsumableDetailsCard(item, itemCard, dc);
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

    public void OpenDetailsMenu(GameObject dim)
    {
        Image image = dim.GetComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0f);
        GameObject panel = dim.transform.GetChild(0).gameObject;
        panel.transform.localScale = Vector3.zero;

        LeanTween.value(dim.gameObject, 0, 0.5f, 0.1f).setEaseLinear().setOnUpdate((float val) =>
        {
            image.color = new Color(0f, 0f, 0f, val);
        }).setOnComplete(() =>
        {
            LeanTween.value(panel, 0, 1f, 0.3f).setEaseInOutCubic().setOnUpdate((float val) =>
            {
                panel.transform.localScale = new Vector3(val, val, val);
            });
        });
    }

    #region Item Card & Item Details Card Modifiers
    // Items Card
    private void ModifyWeaponCard(Item item, bool isEquipped, InventoryCard card)
    {
        if (item is Weapon w)
        {
            card.actionBTN.onClick.AddListener(() => EquipItem(item));

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
    }
    private void ModifyArmorCard(Item item, bool isEquipped, InventoryCard card)
    {
        if (item is Armor a)
        {
            card.actionBTN.onClick.AddListener(() => EquipItem(item));
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
    }
    private void ModifyMaterialCard(Item item, bool isEquipped, InventoryCard card)
    {
        if (item is Material m)
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

                card.detailsBTNText.text = "Details";
            }
        }
    }
    private void ModifyConsumableCard(Item item, bool isEquipped, InventoryCard card)
    {
        if (item is Consumable c)
        {
            card.actionBTN.onClick.AddListener(() => ConsumeItem());
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
    // Item Details Card
    private void ModifyWeaponDetailsCard(Item item, InventoryCard itemCard, DetailsCard dc)
    {
        if (item is Weapon w)
        {
            dc.icon1.sprite = w.WeaponType switch
            {
                WeaponType.Sword => itemCard.swordIcon,
                WeaponType.Bow => itemCard.bowIcon,
                WeaponType.Staff => itemCard.staffIcon,
                _ => itemCard.swordIcon
            }; ;
            dc.value1.text = w.WeaponATK.ToString();

            dc.icon2.sprite = itemCard.levelIcon;
            dc.value2.text = w.RequiredLevel.ToString();

            dc.icon3.sprite = itemCard.upgradeIcon;
            dc.value3.text = $"+{w.Upgrade}";

            dc.icon4.gameObject.SetActive(false);
            dc.value4.gameObject.SetActive(false);
            dc.icon5.gameObject.SetActive(false);
            dc.value5.gameObject.SetActive(false);

            bool isEquipped = context.Player.EquippedWeapon != null && context.Player.EquippedWeapon.ID == w.ID && context.Player.EquippedWeapon.Upgrade == w.Upgrade;
            string text = isEquipped ? "Unequip" : "Equip";

            dc.action.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
            dc.action.onClick.AddListener(() => EquipItem(item));
        }
    }
    private void ModifyArmorDetailsCard(Item item, InventoryCard itemCard, DetailsCard dc)
    {
        if (item is Armor a)
        {
            dc.icon1.sprite = itemCard.armorIcon;
            dc.value1.text = a.ArmorDef.ToString();

            dc.icon2.sprite = itemCard.levelIcon;
            dc.value2.text = a.RequiredLevel.ToString();

            dc.icon3.sprite = itemCard.upgradeIcon;
            dc.value3.text = $"+{a.Upgrade}";

            dc.icon4.sprite = itemCard.hpIcon;
            dc.value4.text = a.ExtraHP.ToString();

            dc.icon5.gameObject.SetActive(false);
            dc.value5.gameObject.SetActive(false);

            dc.action.onClick.AddListener(() => EquipItem(item));
        }
    }
    private void ModifyMaterialDetailsCard(Item item, InventoryCard itemCard, DetailsCard dc)
    {
        if (item is Material m)
        {
            dc.icon1.sprite = itemCard.quantityIcon;
            dc.value1.text = $"x{m.Quantity}";

            dc.icon2.gameObject.SetActive(false);
            dc.value2.gameObject.SetActive(false);
            dc.icon3.gameObject.SetActive(false);
            dc.value3.gameObject.SetActive(false);
            dc.icon4.gameObject.SetActive(false);
            dc.value4.gameObject.SetActive(false);
            dc.icon5.gameObject.SetActive(false);
            dc.value5.gameObject.SetActive(false);
        }
    }
    private void ModifyConsumableDetailsCard(Item item, InventoryCard itemCard, DetailsCard dc)
    {
        if (item is Consumable c)
        {
            //dc.icon1.sprite = itemCard.;
            //dc.value1.text = $"x{m.Quantity}";

            dc.icon1.sprite = itemCard.quantityIcon;
            dc.value1.text = $"x{c.Quantity}";

            dc.action.GetComponent<TMP_Text>().text = "Consume";
            dc.action.onClick.AddListener(() => ConsumeItem());
        }
    }
    #endregion
    
    #region Inventory Actions
    private void EquipItem(Item item)
    {
        Hero p = GameManager.Instance.Context.Player;
        if (item is Weapon w) 
        {
            if (p.EquippedWeapon == w)
            {
                // Unequip
                p.Inventory.Add(new InventoryData
                {
                    InstanceID = System.Guid.NewGuid().ToString(),
                    ID = p.EquippedWeapon.ID,
                    Upgrade = p.EquippedWeapon.Upgrade,
                    Quantity = 1
                });
                p.EquippedWeapon = null;
            }
            else
            {
                // Equip new weapon, first unequip current if any
                if (p.EquippedWeapon != null)
                {
                    p.Inventory.Add(new InventoryData
                    {
                        InstanceID = System.Guid.NewGuid().ToString(),
                        ID = p.EquippedWeapon.ID,
                        Upgrade = p.EquippedWeapon.Upgrade,
                        Quantity = 1
                    });
                }
                p.EquippedWeapon = w;

                // Remove the equipped weapon from inventory
                for (int i = 0; i < p.Inventory.Count; i++)
                {
                    if (p.Inventory[i].ID == w.ID && p.Inventory[i].Upgrade == w.Upgrade)
                    {
                        p.Inventory.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        else if (item is Armor a) 
        {
            if (p.EquippedArmor == a)
            {
                // Unequip
                p.Inventory.Add(new InventoryData
                {
                    InstanceID = System.Guid.NewGuid().ToString(),
                    ID = p.EquippedArmor.ID,
                    Upgrade = p.EquippedArmor.Upgrade,
                    Quantity = 1
                });
                p.EquippedArmor = null;
            }
            else
            {
                // Equip new armor, first unequip current if any
                if (p.EquippedArmor != null)
                {
                    p.Inventory.Add(new InventoryData
                    {
                        InstanceID = System.Guid.NewGuid().ToString(),
                        ID = p.EquippedArmor.ID,
                        Upgrade = p.EquippedArmor.Upgrade,
                        Quantity = 1
                    });
                }
                p.EquippedArmor = a;

                // Remove the equipped armor from inventory
                for (int i = 0; i < p.Inventory.Count; i++)
                {
                    if (p.Inventory[i].ID == a.ID && p.Inventory[i].Upgrade == a.Upgrade)
                    {
                        p.Inventory.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        GenerateItemCards();
        if (inventoryCapacityText != null)
            inventoryCapacityText.text = $"{p.Inventory.Count}/20";
            
        EventManager.HeroEvents.TriggerEquipmentChanged(GameManager.Instance.Context);
    }
    private void ConsumeItem()
    {

    }
    #endregion
}
