using Assets._Project.Scripts.ScriptableObjects.ScriptableObjectScripts;
using TextBasedRPG.Core.Heroes;
using UnityEngine;

namespace Assets._Project.Scripts.UI.Cards.RightSection
{
    public class EquipmentPanels : MonoBehaviour
    {
        [Header("Slots")]
        [SerializeField] private EquipmentSlotUI weaponSlot;
        [SerializeField] private EquipmentSlotUI armorSlot;

        [Header("Rarity Styles")]
        [SerializeField] private RarityDatabase rarityDB;

        public void EquipmentCards(GameContext context)
        {
            Hero p = context.Player;

            weaponSlot.UpdateSlot(p.EquippedWeapon, rarityDB);
            armorSlot.UpdateSlot(p.EquippedArmor, rarityDB);
        }
    }
}
    