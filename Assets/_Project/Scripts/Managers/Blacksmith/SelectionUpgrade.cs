using Assets._Project.Scripts.ScriptableObjects.ScriptableObjectScripts;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Core.Heroes;
using Assets._Project.Scripts.UI;
using TextBasedRPG.Events;

namespace Assets._Project.Scripts.Managers.Blacksmith
{
    [System.Serializable]
    public struct StatRow
    {
        public GameObject root;
        public Image icon;
        public TMP_Text labelText;
        public TMP_Text valueOldText;
        public TMP_Text valueNewText;

        public void SetStat(Sprite statIcon, string label, string valueOld, string valueNew)
        {
            root.SetActive(true);
            if (icon != null) icon.sprite = statIcon;
            labelText.text = label;
            valueOldText.text = valueOld;
            valueNewText.text = valueNew;
        }

        public void Hide() => root.SetActive(false);
    }
    public class SelectionUpgrade : MonoBehaviour
    {
        // -------- Equipment Info
        [SerializeField] private Image equipmentIcon;
        [SerializeField] private TextMeshProUGUI equipmentName;
        [SerializeField] private Outline equipmentOutlineColor;
        [SerializeField] private TextMeshProUGUI equipmentRarityText;
        // -------- Attributes
        [SerializeField] private TextMeshProUGUI oldUpgradeLevel;
        [SerializeField] private TextMeshProUGUI newUpgradeLevel;
        [SerializeField] private StatRow[] statRows;
        // -------- Materials and Cost
        [SerializeField] private TextMeshProUGUI successChance;
        [SerializeField] private TextMeshProUGUI upgradeGoldCost;
        [SerializeField] private Button upgradeBtn;
        // -------- Required Systems
        [SerializeField] public RarityDatabase rarityDB;
        private IconDatabase iconDB;

        private Item _currentItem;
        private Hero _playerHero;
        private int _goldCost;
        private int _successChance;

        private void Start()
        {
            iconDB = UIManager.Instance.IconDB;
            _playerHero = GameManager.Instance.Context.Player;
            
            if (upgradeBtn != null)
            {
                upgradeBtn.onClick.AddListener(OnUpgradeClicked);
            }
        }

        private void OnDestroy()
        {
            if (upgradeBtn != null)
            {
                upgradeBtn.onClick.RemoveListener(OnUpgradeClicked);
            }
        }

        public void Setup(Item item)
        {
            if (this == null) return;
            _currentItem = item;
            RefreshUI();
        }

        private void RefreshUI()
        {
            if (this == null) return;
            if (_currentItem == null) return;
            
            int currentLevel = (_currentItem is Weapon w) ? w.Upgrade : ((_currentItem is Armor a) ? a.Upgrade : 0);
            int maxLevel = UpgradeSystem.GetMaxUpgradeLevel(_currentItem.Rarity);

            if (_currentItem is Weapon we)
            {
                equipmentIcon.sprite = we.WeaponType switch
                {
                    WeaponType.Sword => iconDB.swordIcon,
                    WeaponType.Bow => iconDB.bowIcon,
                    WeaponType.Staff => iconDB.staffIcon,
                    _ => iconDB.questionMarkIcon
                };
            }
            if (_currentItem is Armor ar)
            {
                equipmentIcon.sprite = iconDB.armorIcon;
            }

            equipmentName.text = _currentItem.Name;
            equipmentOutlineColor.effectColor = rarityDB.GetColor(_currentItem.Rarity);

            equipmentRarityText.text = _currentItem.Rarity.ToString();
            equipmentRarityText.color = rarityDB.GetColor(_currentItem.Rarity);

            equipmentName.enableVertexGradient = true;
            equipmentName.colorGradientPreset = rarityDB.GetGradient(_currentItem.Rarity);

            oldUpgradeLevel.text = $"+{currentLevel}";
            
            bool canUpgrade = currentLevel < maxLevel;

            if (canUpgrade)
            {
                _goldCost = UpgradeSystem.CalculateGoldCost(_currentItem.Price, currentLevel, _currentItem.Rarity);
                _successChance = UpgradeSystem.CalculateSuccessChance(currentLevel);

                upgradeGoldCost.text = _goldCost.ToString();
                successChance.text = $"{_successChance}%";

                float chanceFactor = _successChance / 100f;

                successChance.color = Color.Lerp(Color.red, Color.green, chanceFactor);
            }
            else
            {
                upgradeGoldCost.text = "MAX";
                successChance.text = "-";

                successChance.color = Color.white;
            }

            foreach (var row in statRows) row.Hide();

            

            if (_currentItem is Weapon weapon)
            {
                int statCount = 0;
                if (statCount < statRows.Length)
                {
                    int currentAtk = UpgradeSystem.CalculateUpgradedStat(weapon.WeaponATK, weapon.Upgrade);
                    int nextAtk = canUpgrade ? UpgradeSystem.CalculateUpgradedStat(weapon.WeaponATK, weapon.Upgrade + 1) : currentAtk;
                    statRows[statCount].SetStat(iconDB.attackIcon, "ATK", currentAtk.ToString(), canUpgrade ? nextAtk.ToString() : "-");
                    statCount++;
                }
            }
            else if (_currentItem is Armor armor)
            {
                int statCount = 0;
                if (statCount < statRows.Length)
                {
                    int currentDef = UpgradeSystem.CalculateUpgradedStat(armor.ArmorDef, armor.Upgrade);
                    int nextDef = canUpgrade ? UpgradeSystem.CalculateUpgradedStat(armor.ArmorDef, armor.Upgrade + 1) : currentDef;
                    statRows[statCount].SetStat(iconDB.armorIcon, "DEF", currentDef.ToString(), canUpgrade ? nextDef.ToString() : "-");
                    statCount++;
                }
                if (armor.ExtraHP > 0 && statCount < statRows.Length)
                {
                    int currentHP = UpgradeSystem.CalculateUpgradedStat(armor.ExtraHP, armor.Upgrade);
                    int nextHP = canUpgrade ? UpgradeSystem.CalculateUpgradedStat(armor.ExtraHP, armor.Upgrade + 1) : currentHP;
                    statRows[statCount].SetStat(iconDB.hpIcon, "HP", currentHP.ToString(), canUpgrade ? nextHP.ToString() : "-");
                    statCount++;
                }
            }

            if (canUpgrade)
            {
                _goldCost = UpgradeSystem.CalculateGoldCost(_currentItem.Price, currentLevel, _currentItem.Rarity);
                _successChance = UpgradeSystem.CalculateSuccessChance(currentLevel);
                
                upgradeGoldCost.text = _goldCost.ToString();
                successChance.text = $"{_successChance}%";
            }
            else
            {
                upgradeGoldCost.text = "MAX";
                successChance.text = "-";
            }
        }

        private void OnUpgradeClicked()
        {
            if (this == null) return;
            if (_currentItem == null) return;

            int currentLevel = (_currentItem is Weapon w) ? w.Upgrade : ((_currentItem is Armor a) ? a.Upgrade : 0);
            int maxLevel = UpgradeSystem.GetMaxUpgradeLevel(_currentItem.Rarity);

            if (currentLevel >= maxLevel)
            {
                Toaster.Instance.ShowToast("Item is already at max level!", iconDB.lockedIcon);
                return;
            }

            if (_playerHero.Gold < _goldCost)
            {
                Toaster.Instance.ShowToast("Not enough gold!", iconDB.lockedIcon);
                UIExtensions.Shake(upgradeBtn.GetComponent<RectTransform>(), 10f, 0.5f);
                return;
            }

            _playerHero.Gold -= _goldCost;
            
            bool success = UpgradeSystem.RollUpgradeSuccess(currentLevel);

            if (success)
            {
                if (_currentItem is Weapon weapon)
                {
                    weapon.Upgrade++;
                }
                else if (_currentItem is Armor armor)
                {
                    armor.Upgrade++;
                }

                GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);
                Toaster.Instance.ShowToast("Upgrade Successful!", iconDB.confirmIcon);
            }
            else
            {
                Toaster.Instance.ShowToast("Upgrade Failed!", iconDB.lockedIcon);
                UIExtensions.Shake(upgradeBtn.GetComponent<RectTransform>(), 10f, 0.5f);
            }

            RefreshUI();
            EventManager.HeroEvents.TriggerEquipmentChanged(GameManager.Instance.Context);
            EventManager.HeroEvents.TriggerGoldChanged(GameManager.Instance.Context);
        }
    }
}

