using Assets._Project.Scripts.UI.Cards.RightSection;
using TextBasedRPG.Events;
using UnityEngine;

namespace Assets._Project.Scripts.UI.Cards
{
    public class RightSectionManager : MonoBehaviour
    {
        #region References
        [Header("Player Card")]
        [SerializeField] public PlayerPanel playerPanel;

        [Header("Quick Stats")]
        [SerializeField] public QuickStatsPanel quickStatsPanel;

        [Header("Equipment Cards")]
        [SerializeField] public EquipmentPanels equipmentPanels;
        #endregion

        #region Enable & Disable
        private void OnEnable()
        {
            EventManager.HeroEvents.OnEquipmentChanged += UpdateEquipmentUI;
            EventManager.HeroEvents.OnLocationChanged += UpdateRightSection;
            EventManager.HeroEvents.OnTrainingDataChanged += UpdateTrainingUI;
        }

        private void OnDisable()
        {
            EventManager.HeroEvents.OnEquipmentChanged -= UpdateEquipmentUI;
            EventManager.HeroEvents.OnLocationChanged -= UpdateRightSection;
            EventManager.HeroEvents.OnTrainingDataChanged -= UpdateTrainingUI;
        }
        #endregion
        
        #region Event Handlers

        public void UpdateEquipmentUI(GameContext context)
        {
            playerPanel.UpdateHPUI(context);
            quickStatsPanel.PlayerQuickStats(context);
            equipmentPanels.EquipmentCards(context);
        }
        public void UpdateTrainingUI(GameContext context)
        {
            if (context.Player != null)
            {
                playerPanel.UpdateHPUI(context);
                quickStatsPanel.PlayerQuickStats(context);
            }
        }
        #endregion

        #region UI Updates
        public void UpdateRightSection(GameContext context, bool setDelay)
        {
            if (context.Player != null)
            {
                playerPanel.PlayerCard(context);
                quickStatsPanel.PlayerQuickStats(context);
                equipmentPanels.EquipmentCards(context);
            }
        }
        
        #endregion
    }
}
