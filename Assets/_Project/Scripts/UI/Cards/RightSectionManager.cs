using TextBasedRPG.Events;
using TextBasedRPG.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.UI.Cards
{
    public class RightSectionManager : MonoBehaviour
    {
        [Header("Player Card")]
        [SerializeField] private Image playerAvatar;
        [SerializeField] private TMP_Text playerClass;
        [SerializeField] private TMP_Text playerHPValue;
        [SerializeField] private Image playerHPFill;
        [SerializeField] private TMP_Text playerEXPValue;
        [SerializeField] private Image playerEXPFill;
        [SerializeField] private TMP_Text playerLevel;
        [SerializeField] private TMP_Text playerGold;
        [SerializeField] private TMP_Text playerLocation;

        [Header("Equipments")]
        [SerializeField] private TMP_Text equippedWeaponName;
        [SerializeField] private TMP_Text equippedArmorName;

        private void OnEnable()
        {
            EventManager.HeroEvents.OnGoldChanged += UpdateGoldUI;
        }

        private void OnDisable()
        {
            EventManager.HeroEvents.OnGoldChanged -= UpdateGoldUI;
        }

        private void UpdateGoldUI(GameContext context)
        {
            playerGold.text = $"{context.Player.Gold}";

            LeanTween.scale(playerGold.gameObject, Vector3.one * 1.2f, 0.1f).setLoopPingPong(1);
        }

        public void UpdateRightSection(GameContext context)
        {
            if (context.Player != null)
            {
                PlayerCard(context);
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
        private void EquipmentCards(GameContext context)
        {
            Debug.Log(UIManager.Instance.rarityColors["Common"].g);
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
    }
}
