using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TextBasedRPG.Core.Items;
using Assets._Project.Scripts.Managers.Blacksmith;
using Assets._Project.Scripts.ScriptableObjects.ScriptableObjectScripts;

public class UpgradeSelectionCard : MonoBehaviour
{
    [SerializeField] private Image equipmentIcon;
    [SerializeField] private TextMeshProUGUI equipmentName;
    [SerializeField] private TextMeshProUGUI upgradeLevel;
    [SerializeField] private TextMeshProUGUI equipmentLevel;
    [SerializeField] private Button equipmentCard;

    public void Setup(Item item, SelectionUpgrade selectionUpgradeSystem, IconDatabase iconDB)
    {
        if (item == null) return;
        
        if (equipmentName != null) equipmentName.text = item.Name;
        
        if (item is Weapon w)
        {
            if (upgradeLevel != null) upgradeLevel.text = $"UPG. {w.Upgrade}";
            if (equipmentLevel != null) equipmentLevel.text = $"Lv. {w.RequiredLevel}";
            if (equipmentIcon != null)
            {
                equipmentIcon.sprite = w.WeaponType switch
                {
                    WeaponType.Sword => iconDB.swordIcon,
                    WeaponType.Bow => iconDB.bowIcon,
                    WeaponType.Staff => iconDB.staffIcon,
                    _ => iconDB.questionMarkIcon
                };
            }
        }
        else if (item is Armor a)
        {
            if (upgradeLevel != null) upgradeLevel.text = $"UPG. {a.Upgrade}";
            if (equipmentLevel != null) equipmentLevel.text = $"Lv. {a.RequiredLevel}";
            if (equipmentIcon != null) equipmentIcon.sprite = iconDB.armorIcon;
        }
        equipmentIcon.color = selectionUpgradeSystem.rarityDB.GetColor(item.Rarity);

        if (equipmentCard != null && selectionUpgradeSystem != null)
        {
            equipmentCard.onClick.RemoveAllListeners();
            equipmentCard.onClick.AddListener(() =>
            {
                selectionUpgradeSystem.Setup(item);
            });
        }
    }
}
