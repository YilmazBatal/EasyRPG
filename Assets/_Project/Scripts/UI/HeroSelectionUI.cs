using TextBasedRPG.Core.Heroes;
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
                GameManager.Instance.Context.Player = selectedHero; 
                GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);
                GameManager.Instance.ChangeState(GameState.MainMenu);

                UIManager.Instance.rightSection.gameObject.SetActive(true);
                UIManager.Instance.leftSection.gameObject.SetActive(true);

                UIManager.Instance.raycastBlocker1.gameObject.SetActive(false);

                Destroy(UIManager.Instance._activePopUp.gameObject);
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