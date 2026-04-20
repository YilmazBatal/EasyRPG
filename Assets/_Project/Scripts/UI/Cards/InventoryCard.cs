using Assets._Project.Scripts.UI.Cards;
using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Core.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Material = TextBasedRPG.Core.Items.Material;

public class InventoryCard : MonoBehaviour
{
    [Header("Details Prefab")]
    [SerializeField] private GameObject detailsCardPrefab;

    [Header("Components")]
    [SerializeField] public TMP_Text itemName;
    [SerializeField] public Outline itemRarity;
    [SerializeField] public Image itemIcon;
    [SerializeField] public Image subStatIcon1;
    [SerializeField] public TMP_Text subStat1;
    [SerializeField] public Image subStatIcon2;
    [SerializeField] public TMP_Text subStat2;
    [SerializeField] public Image subStatIcon3;
    [SerializeField] public TMP_Text subStat3;
    [SerializeField] public TMP_Text price;
    [SerializeField] public Button actionBTN;
    [SerializeField] public TMP_Text actionBTNText;
    [SerializeField] public Button detailsBTN;
    [SerializeField] public TMP_Text detailsBTNText;

    Hero player => GameManager.Instance.Context.Player;
    IconDatabase iconDB => UIManager.Instance.IconDB;

    public void ModifyItemCard(Item item, bool isEquipped = false)
    {
        // Common properties
        itemName.text = item.Name;
        itemRarity.effectColor = UIManager.Instance.rarityColors[item.Rarity.ToString()];
        price.text = $"{item.Price}G";

        if (detailsBTN != null)
        {
            detailsBTN.interactable = true;
            detailsBTN.onClick.AddListener(() => ShowDetails(item));
        }

        if (actionBTN != null) actionBTN.gameObject.SetActive(true); // ensure it is active by default

        // Specific properties
        ModifyWeaponCard(item, isEquipped);
        ModifyArmorCard(item, isEquipped);
        ModifyMaterialCard(item, isEquipped);
        ModifyConsumableCard(item, isEquipped);
    }
    // Items Card
    #region Modify Item Cards
    private void ModifyWeaponCard(Item item, bool isEquipped)
    {
        if (item is Weapon w)
        {
            actionBTN.onClick.AddListener(() => player.EquipItem(item));

            itemIcon.sprite = w.WeaponType switch
            {
                WeaponType.Sword => iconDB.swordIcon,
                WeaponType.Bow => iconDB.bowIcon,
                WeaponType.Staff => iconDB.staffIcon,
                _ => iconDB.questionMarkIcon
            };

            subStatIcon1.sprite = iconDB.attackIcon;
            subStat1.text = w.WeaponATK.ToString();

            subStatIcon2.sprite = iconDB.levelIcon;
            subStat2.text = w.RequiredLevel.ToString();

            subStatIcon3.sprite = iconDB.upgradeIcon;
            subStat3.text = $"+{w.Upgrade}";

            //detailsBTN
            string text = isEquipped ? "Unequip" : "Equip";
            actionBTNText.text = text;

            if (w.RequiredLevel >= player.Level) // or not enough str etc for the future implementations
            {
                actionBTN.interactable = false;
            }
        }
    }
    private void ModifyArmorCard(Item item, bool isEquipped)
    {
        if (item is Armor a)
        {
            actionBTN.onClick.AddListener(() => player.EquipItem(item));
            itemIcon.sprite = iconDB.armorIcon;

            subStatIcon1.sprite = iconDB.armorIcon;
            subStat1.text = a.ArmorDef.ToString();

            subStatIcon2.sprite = iconDB.levelIcon;
            subStat2.text = a.RequiredLevel.ToString();

            subStatIcon3.sprite = iconDB.upgradeIcon;
            subStat3.text = $"+{a.Upgrade}";

            //detailsBTN
            string text = isEquipped ? "Unequip" : "Equip";
            actionBTNText.text = text;

            if (a.RequiredLevel >= GameManager.Instance.Context.Player.Level) // or not enough str etc for the future implementations
            {
                actionBTN.interactable = false;
            }
        }
    }
    private void ModifyMaterialCard(Item item, bool isEquipped)
    {
        if (item is Material m)
        {
            itemIcon.sprite = iconDB.boneIcon;

            subStatIcon1.sprite = iconDB.quantityIcon;
            subStat1.text = $"x{m.Quantity}";

            subStatIcon2.gameObject.SetActive(false);
            subStat2.gameObject.SetActive(false);

            subStatIcon3.gameObject.SetActive(false);
            subStat3.gameObject.SetActive(false);

            if (actionBTN != null) actionBTN.gameObject.SetActive(false);

            // Make details button full width with 24px padding and set its text to "Details"
            if (detailsBTN != null)
            {
                RectTransform dRt = detailsBTN.GetComponent<RectTransform>();
                if (dRt != null)
                {
                    dRt.anchorMin = new Vector2(0f, dRt.anchorMin.y);
                    dRt.anchorMax = new Vector2(1f, dRt.anchorMax.y);
                    dRt.offsetMin = new Vector2(24f, dRt.offsetMin.y);
                    dRt.offsetMax = new Vector2(-24f, dRt.offsetMax.y);
                }

                detailsBTNText.text = "Details";
            }
        }
    }
    private void ModifyConsumableCard(Item item, bool isEquipped)
    {
        if (item is Consumable c)
        {
            actionBTN.onClick.AddListener(() => player.ConsumeItem());
            itemIcon.sprite = iconDB.meatIcon;

            subStatIcon1.sprite = iconDB.hpIcon;
            subStat1.text = c.Effect;

            subStatIcon2.sprite = iconDB.hammerIcon;
            subStat2.text = c.Value.ToString() + "%";

            subStatIcon3.gameObject.SetActive(false);
            subStat3.gameObject.SetActive(false);

            //detailsBTN
            actionBTNText.text = "Use";
        }
    }
    #endregion
    private void ShowDetails(Item item)
    {
        if (detailsCardPrefab == null)
        {
            Debug.LogError("detailsCardPrefab is missing! Please assign it in the inspector.");
            return;
        }
        //ill fix this funny transform later.
        GameObject detailsObj = Instantiate(detailsCardPrefab, transform.parent.transform.parent.transform.parent.transform.parent.transform);

        detailsObj.transform.GetChild(0).GetComponent<DetailsCard>().OpenDetailsMenu(detailsObj, item, this);
    }


}
