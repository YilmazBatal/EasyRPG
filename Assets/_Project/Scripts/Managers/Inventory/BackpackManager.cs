using TextBasedRPG.Core.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Material = TextBasedRPG.Core.Items.Material;

public class BackpackManager : MonoBehaviour
{
    #region Variables
    [Header("Filter  Buttons")]
    [SerializeField] Button noFilter;
    [SerializeField] Button filterWeapons;
    [SerializeField] Button filterArmors;
    [SerializeField] Button filterConsumables;
    [SerializeField] Button filterMaterials;
    [SerializeField] Button filterMix;

    [Header("Inventory & Item Card Prefab")]
    [SerializeField] TMP_Text inventoryCapacityText;
    [SerializeField] GameObject inventoryContent;
    [SerializeField] GameObject itemCardPrefab;
    
    GameContext context;
    #endregion

    private void OnEnable()
    {
        context = GameManager.Instance.Context;
        GeneateItemCards();
        inventoryCapacityText.text = $"{context.Player.Inventory.Count}/20"; // hard coded 20 for now
    }
    public void GeneateItemCards()
    {
        ClearItems();
        foreach (var item in context.Player.Inventory)
        {
            Item newItem = GetItemWithID(item.ID); // Get id and quantity
            ModifyItemCard(newItem); // Modify the item card with the item data and append to inventory content
        }
    }

    private void ModifyItemCard(Item item)
    {
        GameObject newCard = Instantiate(itemCardPrefab, inventoryContent.transform);
        InventoryCard card = newCard.GetComponent<InventoryCard>();

        // Common properties
        card.itemName.text = item.Name;
        card.itemRarity.effectColor = UIManager.Instance.rarityColors[item.Rarity.ToString()];
        card.price.text = $"{item.Price}G";

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
            card.subStat3.text = $"+{w.Upgrade}"; // hard coded for now

            //card.detailsBTN
            card.actionBTNText.text = "Equip";

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
            card.subStat3.text = $"+0"; // hard coded for now

            //card.detailsBTN
            card.actionBTNText.text = "Equip";

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

            //card.detailsBTN
            card.actionBTNText.text = "Null";

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
}
