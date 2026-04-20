using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Core.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Material = TextBasedRPG.Core.Items.Material;

namespace Assets._Project.Scripts.UI.Cards
{
    public class DetailsCard : MonoBehaviour
    {
        [SerializeField] public Image background;

        [SerializeField] public TMP_Text title;
        [SerializeField] public TMP_Text desc;
        [SerializeField] public Image itemIcon;

        [SerializeField] public Image icon1;
        [SerializeField] public TMP_Text value1;
        [SerializeField] public Image icon2;
        [SerializeField] public TMP_Text value2;
        [SerializeField] public Image icon3;
        [SerializeField] public TMP_Text value3;
        [SerializeField] public Image icon4;
        [SerializeField] public TMP_Text value4;
        [SerializeField] public Image icon5;
        [SerializeField] public TMP_Text value5;

        [SerializeField] public TMP_Text gold;

        public Button action;
        public Button closeBTN;

        Hero player => GameManager.Instance.Context.Player;
        IconDatabase iconDB => UIManager.Instance.IconDB;

        private void OnEnable()
        {
            closeBTN.onClick.RemoveAllListeners();
            closeBTN.onClick.AddListener(() => CloseDetailsMenu(background.gameObject));
        }

        void SetUpDetailsCard(Item item, InventoryCard itemCard)
        {
            title.text = item.Name;
            desc.text = item.Description;
            gold.text = $"{item.Price}G";
            itemIcon.sprite = itemCard.itemIcon.sprite;
            itemIcon.transform.parent.GetComponent<Outline>().effectColor = UIManager.Instance.rarityColors[item.Rarity.ToString()];

            ModifyWeaponDetailsCard(item, itemCard);
            ModifyArmorDetailsCard(item, itemCard);
            ModifyMaterialDetailsCard(item, itemCard);
            ModifyConsumableDetailsCard(item, itemCard);
        }

        #region Item Details Card Modifiers
        private void ModifyWeaponDetailsCard(Item item, InventoryCard itemCard)
        {
            if (item is Weapon w)
            {
                icon1.sprite = iconDB.GetWeaponIcon(w.WeaponType);
                value1.text = w.WeaponATK.ToString();

                icon2.sprite = iconDB.levelIcon;
                value2.text = w.RequiredLevel.ToString();

                icon3.sprite = iconDB.upgradeIcon;
                value3.text = $"+{w.Upgrade}";

                icon4.gameObject.SetActive(false);
                value4.gameObject.SetActive(false);
                icon5.gameObject.SetActive(false);
                value5.gameObject.SetActive(false);

                bool isEquipped = player.EquippedWeapon != null && player.EquippedWeapon.ID == w.ID && player.EquippedWeapon.Upgrade == w.Upgrade;
                string text = isEquipped ? "Unequip" : "Equip";

                action.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
                action.onClick.AddListener(() => player.EquipItem(item));
            }
        }
        private void ModifyArmorDetailsCard(Item item, InventoryCard itemCard)
        {
            if (item is Armor a)
            {
                icon1.sprite = iconDB.armorIcon;
                value1.text = a.ArmorDef.ToString();

                icon2.sprite = iconDB.levelIcon;
                value2.text = a.RequiredLevel.ToString();

                icon3.sprite = iconDB.upgradeIcon;
                value3.text = $"+{a.Upgrade}";

                icon4.sprite = iconDB.hpIcon;
                value4.text = a.ExtraHP.ToString();

                icon5.gameObject.SetActive(false);
                value5.gameObject.SetActive(false);

                action.onClick.AddListener(() => player.EquipItem(item));
            }
        }
        private void ModifyMaterialDetailsCard(Item item, InventoryCard itemCard)
        {
            if (item is Material m)
            {
                icon1.sprite = iconDB.quantityIcon;
                value1.text = $"x{m.Quantity}";

                icon2.gameObject.SetActive(false);
                value2.gameObject.SetActive(false);
                icon3.gameObject.SetActive(false);
                value3.gameObject.SetActive(false);
                icon4.gameObject.SetActive(false);
                value4.gameObject.SetActive(false);
                icon5.gameObject.SetActive(false);
                value5.gameObject.SetActive(false);
            }
        }
        private void ModifyConsumableDetailsCard(Item item, InventoryCard itemCard)
        {
            if (item is Consumable c)
            {
                //icon1.sprite = itemCard.;
                //value1.text = $"x{m.Quantity}";

                icon1.sprite = iconDB.quantityIcon;
                value1.text = $"x{c.Quantity}";

                action.GetComponent<TMP_Text>().text = "Consume";
                action.onClick.AddListener(() => player.ConsumeItem());
            }
        }
        #endregion

        #region Open And Close Details Menu
        public void OpenDetailsMenu(GameObject dim, Item item, InventoryCard itemCard)
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

            SetUpDetailsCard(item, itemCard);
        }
        private void CloseDetailsMenu(GameObject panel)
        {
            closeBTN.interactable = false;

            LeanTween.value(panel.gameObject, 1f, 0f, 0.3f).setEaseInOutCubic().setOnUpdate((float val) =>
            {
                panel.transform.localScale = new Vector3(val, val, val);
            }).setOnComplete(() =>
            {
                GameObject shadow = panel.gameObject.transform.parent.gameObject;
                Image image = shadow.GetComponent<Image>();
                LeanTween.value(shadow, image.color.a, 0f, 0.1f).setEaseLinear().setOnUpdate((float val) =>
                {
                    shadow.GetComponent<Image>().color = new Color(0f, 0f, 0f, val);
                }).setOnComplete(() => Destroy(shadow.gameObject));
            });
        }
        #endregion

    }
}
