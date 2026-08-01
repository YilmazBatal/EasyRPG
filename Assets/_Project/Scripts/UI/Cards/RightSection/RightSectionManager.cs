using TextBasedRPG.Events;
using TextBasedRPG.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.UI.Cards
{
    public class RightSectionManager : MonoBehaviour
    {
        #region References
        [Header("Player Card")]
        [SerializeField] public PlayerPanel playerPanel;

        [Header("Quick Stats")]
        [SerializeField] public QuickStatsPanel quickStatsPanel;

        [Header("Equipments")]
        [SerializeField] private TMP_Text equippedWeaponName;
        [SerializeField] private TMP_Text equippedArmorName;
        #endregion

        #region Enable & Disable
        private void OnEnable()
        {
            EventManager.HeroEvents.OnEquipmentChanged += UpdateEquipmentUI;
            EventManager.HeroEvents.OnLocationChanged += UpdateRightSection;
        }

        private void OnDisable()
        {
            EventManager.HeroEvents.OnEquipmentChanged -= UpdateEquipmentUI;
            EventManager.HeroEvents.OnLocationChanged -= UpdateRightSection;
        }
        #endregion
        
        #region Event Handlers

        public void UpdateEquipmentUI(GameContext context)
        {
            quickStatsPanel.PlayerQuickStats(context);
            EquipmentCards(context);
            playerPanel.UpdateHPUI(context);
        }
        #endregion

        #region UI Updates
        public void UpdateRightSection(GameContext context, bool setDelay)
        {
            if (context.Player != null)
            {
                playerPanel.PlayerCard(context);
                quickStatsPanel.PlayerQuickStats(context);
                EquipmentCards(context);
            }
        }
        
        private void EquipmentCards(GameContext context)
        {
            if (context.Player.EquippedWeapon != null)
            {
                equippedWeaponName.text = context.Player.EquippedWeapon.Name;
                equippedWeaponName.color = UIManager.Instance.rarityColors[context.Player.EquippedWeapon.Rarity.ToString()];
            } else
            {
                equippedWeaponName.text = "None";
                equippedWeaponName.color = UIManager.Instance.rarityColors["Common"];
            }
            if (context.Player.EquippedArmor != null)
            {
                equippedArmorName.text = context.Player.EquippedArmor.Name;
                equippedArmorName.color = UIManager.Instance.rarityColors[context.Player.EquippedArmor.Rarity.ToString()];
            }
            else
            {
                equippedArmorName.text = "None";
                equippedArmorName.color = UIManager.Instance.rarityColors["Common"];
            }
        }
        #endregion
    }
}
