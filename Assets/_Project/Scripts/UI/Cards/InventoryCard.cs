using Assets._Project.Scripts.UI.Cards;
using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Material = TextBasedRPG.Core.Items.Material;
using Assets._Project.Scripts.Enums;
using Assets._Project.Scripts.ScriptableObjects.ScriptableObjectScripts;

public class InventoryCard : MonoBehaviour
{
    [Header("Details Prefab")]
    [SerializeField] private GameObject detailsCardPrefab;

    [Header("Rarity Styles")]
    [SerializeField] private RarityDatabase rarityDB;

    [Header("Components")]
    [SerializeField] public TMP_Text itemName;
    [SerializeField] public Outline itemRarity;
    [SerializeField] public TMP_Text rarityText;
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
    [SerializeField] public Image detailsBTNIcon;
    [SerializeField] public TMP_Text detailsBTNText;
    [SerializeField] public Button discardBTN;

    Hero player => GameManager.Instance.Context.Player;
    IconDatabase iconDB => UIManager.Instance.IconDB;

    // discard confirmation state
    private bool _awaitingDiscardConfirm = false;
    private Coroutine _discardCoroutine = null;

    public void ModifyItemCard(Item item, bool isEquipped = false)
    {
        // Common properties
        itemName.text = item.Name;
        rarityText.text = item.Rarity.ToString();
        rarityText.color = rarityDB.GetColor(item.Rarity);
        itemRarity.effectColor = rarityDB.GetColor(item.Rarity);
        price.text = $"{item.Price}G";

        if (actionBTN != null)
            actionBTN.gameObject.SetActive(true);
        
        if (detailsBTN != null)
        {
            detailsBTN.interactable = true;
            detailsBTN.onClick.AddListener(() => ShowDetails(item));
        }

        if (discardBTN != null)
        {
            discardBTN.interactable = true;
            discardBTN.onClick.AddListener(() => QuickDiscard(item));
        }


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

            if (w.RequiredLevel > player.Level) // or not enough str etc for the future implementations
                actionBTN.interactable = false;
            else
                actionBTN.interactable = true;

            Debug.Log($"Weapon: {w.Name}, Required Level: {w.RequiredLevel}, Player Level: {player.Level}");
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

            if (a.RequiredLevel > player.Level) // or not enough str etc for the future implementations
                actionBTN.interactable = false;
            else
                actionBTN.interactable = true;
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
                detailsBTNText.text = "Details";
                //Destroy(detailsBTNIcon);
                RectTransform dRt = detailsBTN.GetComponent<RectTransform>();
                if (dRt != null)
                {
                    dRt.anchorMin = new Vector2(0f, dRt.anchorMin.y);
                    dRt.anchorMax = new Vector2(1f, dRt.anchorMax.y);
                    dRt.offsetMin = new Vector2(24f, dRt.offsetMin.y);
                    dRt.offsetMax = new Vector2(-48-12f, dRt.offsetMax.y);
                }
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
    private void QuickDiscard(Item item)
    {
        if (discardBTN == null) return;

        Image discImg = discardBTN.GetComponent<Image>();

        // First click: switch to confirm icon and start timeout
        if (!_awaitingDiscardConfirm)
        {
            _awaitingDiscardConfirm = true;
            discImg.transform.GetChild(0).GetComponent<Image>().sprite = iconDB.confirmIcon;
            discImg.transform.GetChild(0).GetComponent<Image>().color = rarityDB.GetColor(Rarity.Uncommon);

            // start timeout to revert icon after 3 seconds
            if (_discardCoroutine != null) StopCoroutine(_discardCoroutine);
            _discardCoroutine = StartCoroutine(DiscardConfirmTimeout(discImg));
            return;
        }

        // Second click within 3 seconds: perform discard
        if (_awaitingDiscardConfirm)
        {
            _awaitingDiscardConfirm = false;
            if (_discardCoroutine != null) { StopCoroutine(_discardCoroutine); _discardCoroutine = null; }

            discImg.transform.GetChild(0).GetComponent<Image>().sprite = iconDB.trashIcon;

            // Remove one quantity via centralized InventoryManager
            InventoryManager.RemoveFromInventory(item.ID, 1);

            // Shake the containing panel
            RectTransform panelRect = GetComponentInParent<RectTransform>();
            if (panelRect != null)
            {
                Assets._Project.Scripts.UI.UIExtensions.Shake(panelRect, 8f, 0.45f);
            }

            return;
        }
    }

    private IEnumerator DiscardConfirmTimeout(Image discImg)
    {
        yield return new WaitForSeconds(3f);
        _awaitingDiscardConfirm = false;
        _discardCoroutine = null;
        discImg.transform.GetChild(0).GetComponent<Image>().sprite = iconDB.trashIcon;
        discImg.transform.GetChild(0).GetComponent<Image>().color = Color.red;
    }


}
