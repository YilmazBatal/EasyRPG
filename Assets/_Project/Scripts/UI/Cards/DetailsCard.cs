using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Material = TextBasedRPG.Core.Items.Material;

namespace Assets._Project.Scripts.UI.Cards
{
    public class DetailsCard : MonoBehaviour
    {
        [SerializeField] public Image background;

        [Header("Main Components")]
        [SerializeField] public TMP_Text title;
        [SerializeField] public TMP_Text desc;
        [SerializeField] public Image itemIcon;

        [Header("Other Components")]
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

        [Header("Buttons")]
        [SerializeField] public Button action;
        [SerializeField] public Button closeBTN;
        [SerializeField] public Button discardBTN;

        [Header("Discard Confirmation")]
        [SerializeField] public TMP_InputField amountText;
        [SerializeField] public Button discardPlusBTN;
        [SerializeField] public Button discardMinusBTN;
        [SerializeField] public Button proceedBTN;

        [Header("Popup")]
        [SerializeField] public GameObject popup;


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

            // Initialize discard state so +/- buttons work immediately
            currentItem = item;
            currentDiscardAmount = 1;
            maxDiscardAmount = item.Quantity;
            var context = GameManager.Instance.Context;
            var entry = context.Player.Inventory.Find(x => x.ID == item.ID);
            if (entry != null) maxDiscardAmount = Mathf.Max(1, entry.Quantity);
            amountText.text = currentDiscardAmount.ToString();

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
                var actionLabel = action.transform.GetChild(0).GetComponent<TMP_Text>();
                actionLabel.text = isEquipped ? "Unequip" : "Equip";

                action.onClick.RemoveAllListeners();
                action.onClick.AddListener(() =>
                {
                    player.EquipItem(item);
                    // Toggle button text based on new equipped state
                    bool nowEquipped = player.EquippedWeapon != null && player.EquippedWeapon.ID == w.ID && player.EquippedWeapon.Upgrade == w.Upgrade;
                    actionLabel.text = nowEquipped ? "Unequip" : "Equip";
                });

                DisableDiscardSystem(discardPlusBTN, discardMinusBTN, proceedBTN, amountText);

                if (w.RequiredLevel >= GameManager.Instance.Context.Player.Level) // or not enough str etc for the future implementations
                    action.interactable = false;

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

                bool isEquipped = player.EquippedArmor != null && player.EquippedArmor.ID == a.ID && player.EquippedArmor.Upgrade == a.Upgrade;
                var actionLabel = action.transform.GetChild(0).GetComponent<TMP_Text>();
                actionLabel.text = isEquipped ? "Unequip" : "Equip";

                action.onClick.RemoveAllListeners();
                action.onClick.AddListener(() =>
                {
                    player.EquipItem(item);
                    // Toggle button text based on new equipped state
                    bool nowEquipped = player.EquippedArmor != null && player.EquippedArmor.ID == a.ID && player.EquippedArmor.Upgrade == a.Upgrade;
                    actionLabel.text = nowEquipped ? "Unequip" : "Equip";
                    GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);
                });

                DisableDiscardSystem(discardPlusBTN, discardMinusBTN, proceedBTN, amountText);

                if (a.RequiredLevel >= GameManager.Instance.Context.Player.Level) // or not enough str etc for the future implementations
                    action.interactable = false;
            }
        }
        private void ModifyMaterialDetailsCard(Item item, InventoryCard itemCard)
        {
            if (item is Material m)
            {
                icon1.sprite = iconDB.quantityIcon;
                value1.text = $"x{m.Quantity}";

                action.gameObject.SetActive(false);

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
                icon1.sprite = iconDB.quantityIcon;
                value1.text = $"x{c.Quantity}";

                var actionText = action.transform.GetChild(0).GetComponent<TMP_Text>();
                actionText.text = "Consume";
                action.onClick.RemoveAllListeners();
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
        #region Discard System

        private Item currentItem;
        private int currentDiscardAmount;
        private int maxDiscardAmount;


        private void UpdateDiscardAmountDisplay()
        {
            amountText.text = currentDiscardAmount.ToString();
        }

        public void ReduceAmount()
        {
            if (currentDiscardAmount > 1)
            {
                Debug.Log($"Decreasing discard amount: {currentDiscardAmount} -> {currentDiscardAmount - 1}");
                currentDiscardAmount--;
                UpdateDiscardAmountDisplay();
            }
        }

        public void IncreaseAmount()
        {
            if (currentDiscardAmount < maxDiscardAmount)
            {
                Debug.Log($"Increasing discard amount: {currentDiscardAmount} -> {currentDiscardAmount + 1}");
                currentDiscardAmount++;
                UpdateDiscardAmountDisplay();
            }
        }

        /// <summary>
        /// Called when the Proceed (save-icon) button is clicked.
        /// Reads the current amount and shows a confirmation popup.
        /// </summary>
        public void ProceedDiscard()
        {
            if (currentItem == null) return;

            // Read amount from input field in case user typed a value directly
            if (int.TryParse(amountText.text, out int typedAmount))
            {
                currentDiscardAmount = Mathf.Clamp(typedAmount, 1, maxDiscardAmount);
            }

            if (currentDiscardAmount <= 0) return;

            Debug.Log($"Proceeding to discard {currentDiscardAmount} of {currentItem.Name}");

            UIManager.Instance.GeneratePopUp(
                "Discard Item",
                $"Are you sure you want to discard {currentDiscardAmount}x {currentItem.Name}?",
                ExecuteDiscard
            );
        }

        /// <summary>
        /// Actually removes items from the inventory. Called after user confirms the popup.
        /// </summary>
        private void ExecuteDiscard()
        {
            InventoryManager.RemoveFromInventory(
                currentItem.ID,
                currentDiscardAmount
            );

            // Close the confirmation popup
            if (UIManager.Instance._activePopUp != null)
            {
                UIManager.Instance._activePopUp.GetComponent<PopUpManager>().Close();
            }

            // Hide the discard amount picker
            ResetDiscardState();

            // Close the details card entirely
            CloseDetailsMenu(background.gameObject);
        }
        public void InputUpdated()
        {
            if (int.TryParse(amountText.text, out int value))
            {
                currentDiscardAmount = Mathf.Clamp(value, 1, maxDiscardAmount);
                UpdateDiscardAmountDisplay();
            }
        }

        private void ResetDiscardState()
        {
            currentItem = null;
            currentDiscardAmount = 1;
            maxDiscardAmount = 0;

            if (popup != null) popup.SetActive(false);
        }

        #endregion

        void DisableDiscardSystem(Button incrementBTN, Button reducementBTN, Button proceedBTN, TMP_InputField amountInput)
        {
            incrementBTN.gameObject.SetActive(false);
            reducementBTN.gameObject.SetActive(false);
            proceedBTN.gameObject.SetActive(false);
            amountInput.gameObject.SetActive(false);
        }
    }
}
