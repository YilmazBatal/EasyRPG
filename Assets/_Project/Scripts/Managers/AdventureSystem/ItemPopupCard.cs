using Assets._Project.Scripts.Managers.AdventureSystem;
using Assets._Project.Scripts.UI;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Core.Locations;
using TextBasedRPG.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPopupCard : MonoBehaviour
{
    #region Variables
    [Header("Animations")]
    [SerializeField] Image dim;

    [Header("Components")]
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text desc;
    [SerializeField] Image icon;
    [SerializeField] Outline rarityColor;
    [SerializeField] Image icon1;
    [SerializeField] TMP_Text val1;
    [SerializeField] Image icon2;
    [SerializeField] TMP_Text val2;
    [SerializeField] Image icon3;
    [SerializeField] TMP_Text val3;
    [SerializeField] Image arrow1;
    [SerializeField] Image arrow2;
    [SerializeField] Image arrow3;
    [SerializeField] Button claim;
    [SerializeField] Button equip;
    [SerializeField] Button discard;

    GameContext context;
    #endregion
    private void OnEnable()
    {
        context = GameManager.Instance.Context;
        OpenAnimation();
    }
    void SetUpCard()
    {
        Location loc = LocationManager.GetLocationByID(context.Player.ActiveLocation);
        LootResult ls = LootManager.AdventureLootGenerator(loc);
        Item item = LootManager.FindItemByID(ls.ID);
        IconDatabase iconDB = UIManager.Instance.IconDB;
        title.text = item.Name;
        desc.text = item.Description;
        rarityColor.effectColor = UIManager.Instance.rarityColors[item.Rarity.ToString()];
        claim.onClick.RemoveAllListeners();
        claim.onClick.AddListener(() => Claim(ls));
        equip.onClick.RemoveAllListeners();
        equip.onClick.AddListener(() => ChangeEquipment(ls));
        if (item is Weapon w)
        {
            icon.sprite = iconDB.GetWeaponIcon(w.WeaponType);
            icon1.sprite = iconDB.attackIcon;
            val1.text = w.WeaponATK.ToString();
            icon2.sprite = iconDB.levelIcon;
            val2.text = w.RequiredLevel.ToString();
            icon3.gameObject.SetActive(false);
            val3.gameObject.SetActive(false);
            arrow3.gameObject.SetActive(false);

            if (context.Player.EquippedWeapon!= null) {

                if (w.WeaponATK < context.Player.EquippedWeapon.WeaponATK)
                {
                    arrow1.sprite = iconDB.downIcon;
                    arrow1.color = Color.red;
                } else
                {
                    arrow1.sprite = iconDB.upIcon;
                    arrow1.color = Color.green;
                }
                if (w.RequiredLevel < context.Player.EquippedWeapon.RequiredLevel)
                {
                    arrow2.sprite = iconDB.downIcon;
                    arrow2.color = Color.red;
                } else
                {
                    arrow2.sprite = iconDB.upIcon;
                    arrow2.color = Color.green;
                }
            }
        }
        else if (item is Armor a)
        {
            icon.sprite = iconDB.armorIcon;
            icon1.sprite = iconDB.armorIcon;
            val1.text = a.ArmorDef.ToString();
            icon2.sprite = iconDB.levelIcon;
            val2.text = a.RequiredLevel.ToString();
            icon3.sprite = iconDB.plusIcon;
            val3.text = a.ExtraHP.ToString();

            if (context.Player.EquippedArmor != null)
            {


                if (a.ArmorDef < context.Player.EquippedArmor.ArmorDef)
                {
                    arrow1.sprite = iconDB.downIcon;
                    arrow1.color = Color.red;
                }
                else
                {
                    arrow1.sprite = iconDB.upIcon;
                    arrow1.color = Color.green;
                }
                if (a.RequiredLevel < context.Player.EquippedArmor.RequiredLevel)
                {
                    arrow2.sprite = iconDB.downIcon;
                    arrow2.color = Color.red;
                }
                else
                {
                    arrow2.sprite = iconDB.upIcon;
                    arrow2.color = Color.green;
                }
                if (a.ExtraHP < context.Player.EquippedArmor.ExtraHP)
                {
                    arrow2.sprite = iconDB.downIcon;
                    arrow2.color = Color.red;
                }
                else
                {
                    arrow2.sprite = iconDB.upIcon;
                    arrow2.color = Color.green;
                }
            }
        }
        else if (item is TextBasedRPG.Core.Items.Material m)
        {
            icon.sprite = iconDB.boneIcon;
            icon1.sprite = iconDB.quantityIcon;
            val1.text = m.Quantity.ToString();

            icon2.gameObject.SetActive(false);
            val2.gameObject.SetActive(false);
            icon3.gameObject.SetActive(false);
            val3.gameObject.SetActive(false);

            arrow1.gameObject.SetActive(false);
            arrow2.gameObject.SetActive(false);
            arrow3.gameObject.SetActive(false);

            equip.gameObject.SetActive(false);

        }
        else if (item is Consumable c)
        {
            icon.sprite = iconDB.potIcon;
            icon1.sprite = iconDB.plusIcon;
            val1.text = c.Effect.ToString();
            
            icon2.sprite = iconDB.upgradeIcon;
            val2.text = c.Value.ToString();

            icon3.gameObject.SetActive(false);
            val3.gameObject.SetActive(false);

            arrow1.gameObject.SetActive(false);
            arrow2.gameObject.SetActive(false);
            arrow3.gameObject.SetActive(false);

            equip.gameObject.SetActive(false);

        }

    }
    #region Button Actions
    public void Claim(LootResult ls)
    {
        // add condition here about inventory space later
        InventoryManager.AddToInventory(ls);
        CloseAnimation();
    }

    public void ChangeEquipment(LootResult ls)
    {
        // add condition here about inventory space later
        Item item = LootManager.FindItemByID(ls.ID);
        GameManager.Instance.Context.Player.EquipItem(item);
        CloseAnimation();
    }
    #endregion

    #region Open And Close Details Menu
    public void OpenAnimation()
    {
        UIExtensions.Shake(gameObject.GetComponent<RectTransform>(), 4f, 1.5f);
        dim.color = new Color(dim.color.r, dim.color.g, dim.color.b, 0);
        gameObject.transform.localScale = Vector3.zero;

        // get the dim light up then with 2nd LT make the panel scale original size.
        LeanTween.cancel(dim.gameObject);
        LeanTween.value(dim.gameObject, 0, 0.5f, 0.1f).setEaseLinear().setOnUpdate((float val) =>
        {
            dim.color = new Color(0f, 0f, 0f, val);
        }).setOnComplete(() =>
        {
            LeanTween.value(dim.gameObject, 0, 1f, 0.5f).setEaseInOutCubic().setOnUpdate((float val) =>
            {
                gameObject.transform.localScale = new Vector3(val, val, val);
            });
        });

        SetUpCard();
    }
    public void CloseAnimation()
    {
        LeanTween.cancel(dim.gameObject);
        LeanTween.value(dim.gameObject, 1f, 0f, 0.5f).setEaseInOutCubic().setOnUpdate((float val) =>
        {
            gameObject.transform.localScale = new Vector3(val, val, val);
        }).setOnComplete(() =>
        {
            
            LeanTween.value(dim.gameObject, 0.5f, 0f, 0.1f).setEaseLinear().setOnUpdate((float val) =>
            {
                dim.color = new Color(0f, 0f, 0f, val);
            }).setOnComplete(() =>
            {
                dim.gameObject.SetActive(false);
                //adventureManager.
            });
        });
    }
    #endregion
}
