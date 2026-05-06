using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.UI
{
    public class HeroSelectionUI : MonoBehaviour
    {
        [Header("Hero Configuration")]
        [SerializeField] public HeroData heroData;

        private Button _button;

        void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            UIManager.Instance.GeneratePopUp(
                heroData.className,
                heroData.description,
                () => ConfirmSelection()
            );
        }

        private void ConfirmSelection()
        {
            Hero selectedHero = CreateHeroByClass(heroData);

            if (selectedHero != null)
            {
                selectedHero.ActiveLocation = "L001";
                selectedHero.UnlockedUntill = 1;
                selectedHero.Gold = 100;

                GameManager.Instance.Context.Player = selectedHero; 
                selectedHero.FullHeal();
                GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);

                // Make UI sections visible before triggering events so subscribers can update them
                UIManager.Instance.rightSection.gameObject.SetActive(true);
                UIManager.Instance.leftSection.gameObject.SetActive(true);

                UIManager.Instance.raycastBlocker1.gameObject.SetActive(false);

                Destroy(UIManager.Instance._activePopUp.gameObject);

                // Switch to MainMenu panel
                GameManager.Instance.ChangeState(GameState.MainMenu);

                // Trigger location event to initialize backgrounds and right section UI
                // (replicates what normally happens on startup when a save already exists)
                EventManager.HeroEvents.TriggerLocationChanged(GameManager.Instance.Context, false);
            }
        }

        private Hero CreateHeroByClass(HeroData data)
        {
            return data.className switch
            {
                "Warrior" => new Warrior(data),
                "Archer" => new Archer(data),
                "Mage" => new Mage(data),
                _ => null
            };
        }
    }
}