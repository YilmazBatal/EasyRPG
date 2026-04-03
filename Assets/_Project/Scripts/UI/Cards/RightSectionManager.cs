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
        [SerializeField] public Image playerAvatar;
        [SerializeField] private TMP_Text playerClass;
        [SerializeField] private TMP_Text playerHPValue;
        [SerializeField] private Image playerHPFill;
        [SerializeField] private TMP_Text playerEXPValue;
        [SerializeField] private Image playerEXPFill;
        [SerializeField] private TMP_Text playerLevel;
        [SerializeField] private TMP_Text playerGold;
        [SerializeField] private TMP_Text playerLocation;
        [SerializeField] public TMP_Text damageText;
        [Header("Quick Stats")]
        [SerializeField] public TMP_Text attackVal;
        [SerializeField] public TMP_Text defenseVal;
        [SerializeField] public TMP_Text speedVal;

        [Header("Equipments")]
        [SerializeField] private TMP_Text equippedWeaponName;
        [SerializeField] private TMP_Text equippedArmorName;
        #endregion

        #region Enable & Disable
        private void OnEnable()
        {
            EventManager.HeroEvents.OnGoldChanged += UpdateGoldUI;
            EventManager.HeroEvents.OnExpChanged += UpdatExpUI;
            EventManager.HeroEvents.OnHPValueChanged += UpdateHPUI;
        }

        private void OnDisable()
        {
            EventManager.HeroEvents.OnGoldChanged -= UpdateGoldUI;
            EventManager.HeroEvents.OnExpChanged -= UpdatExpUI;
            EventManager.HeroEvents.OnHPValueChanged -= UpdateHPUI;
        }
        #endregion
        
        #region Event Handlers
        public void UpdateGoldUI(GameContext context)
        {
            playerGold.text = $"{context.Player.Gold}";

            LeanTween.scale(playerGold.gameObject, Vector3.one * 1.2f, 0.1f).setLoopPingPong(1);
        }
        public void UpdatExpUI(GameContext context)
        {
            playerLevel.text = $"{context.Player.Level}";
            playerEXPValue.text = $"{context.Player.CurExp}/{context.Player.ReqExp}";

            LeanTween.value(playerEXPFill.gameObject, playerEXPFill.fillAmount, (float)context.Player.CurExp / context.Player.ReqExp, 0.5f).setOnUpdate((float val) =>
            {
                playerEXPFill.fillAmount = val;
            }).setEaseInOutCubic();

            LeanTween.scale(playerEXPValue.gameObject, Vector3.one * 1.2f, 0.1f).setLoopPingPong(1);
            LeanTween.scale(playerEXPFill.transform.parent.transform.parent.gameObject, Vector3.one * 1.1f, 0.1f).setLoopPingPong(1);
        }
        public void UpdateHPUI(GameContext context)
        {
            playerHPValue.text = $"{context.Player.CurHP}/{context.Player.TotalHP}";

            LeanTween.value(playerHPFill.gameObject, playerHPFill.fillAmount, (float)context.Player.CurHP / context.Player.TotalHP, 0.5f).setOnUpdate((float val) =>
            {
                playerHPFill.fillAmount = val;
            }).setEaseInOutCubic();

            LeanTween.scale(playerHPValue.gameObject, Vector3.one * 1.2f, 0.1f).setLoopPingPong(1);
            LeanTween.scale(playerHPFill.transform.parent.transform.parent.gameObject, Vector3.one * 1.1f, 0.1f).setLoopPingPong(1);

        }
        #endregion
        
        #region UI Updates
        public void UpdateRightSection(GameContext context)
        {
            if (context.Player != null)
            {
                PlayerCard(context);
                PlayerQuickStats(context);
                EquipmentCards(context);
            }
        }
        private void PlayerCard(GameContext context)
        {
            var heroDb = Resources.Load<HeroDatabase>("HeroDatabase");

            playerAvatar.sprite = heroDb.GetHeroByName(context.Player.ClassName).classIcon;
            playerClass.text = context.Player.ClassName;
            
            playerHPValue.text = $"{context.Player.CurHP}/{context.Player.TotalHP}";
            playerHPFill.fillAmount = (float)context.Player.CurHP / context.Player.TotalHP;
            
            playerEXPValue.text = $"{context.Player.CurExp}/{context.Player.ReqExp}";
            playerEXPFill.fillAmount = (float)context.Player.CurExp / context.Player.ReqExp;

            playerLevel.text = $"{context.Player.Level}";
            UpdateGoldUI(context);

            if (context.Player.ActiveLocation != null)
                playerLocation.text = LocationManager.locations[context.Player.ActiveLocation];
            else
                playerLocation.text = "Unknown";
        }
        private void PlayerQuickStats(GameContext context)
        {
            attackVal.text = $"{context.Player.TotalATK}";
            defenseVal.text = $"{context.Player.TotalDEF}";
            speedVal.text = $"{context.Player.TotalSPD}";
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
