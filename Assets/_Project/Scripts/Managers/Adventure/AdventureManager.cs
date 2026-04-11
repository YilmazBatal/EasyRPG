using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.Managers.Adventure
{
    public class AdventureManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] TMP_Text adventureText;
        [SerializeField] Button adventureButton;
        [SerializeField] Image cooldownBar;
        [SerializeField] GameObject combatPopup;
        [SerializeField] GameObject itemPopup;
        [SerializeField] int cooldown;

        [Header("Chances")]
        [Range(0, 100)] public int adventureTextChance = 60;
        [Range(0, 100)] public int enemyEncounterChance = 30;
        [Range(0, 100)] public int itemDropChance = 10;

        Coroutine activeRoutine;

        public void OnAdventureButtonClicked()
        {
            ManageButtonCooldown();
            ProcessRandomEvent();
        }

        private void ProcessRandomEvent()
        {
            if (activeRoutine != null) StopCoroutine(activeRoutine);
            activeRoutine = null;

            int roll = Random.Range(0, 101);

            if (roll <= itemDropChance)
                adventureText.text = "Item dropped";
            else if (roll <= enemyEncounterChance)
                GenerateArena();
            else
                GenerateAdventureText();
        }
        private void ManageButtonCooldown()
        {
            adventureButton.interactable = false;
            LeanTween.cancel(cooldownBar.gameObject);
            LeanTween.value(cooldownBar.gameObject, 1f, 0f, cooldown).setOnUpdate((float val) =>
            {
                cooldownBar.fillAmount = val;
            }).setEaseInOutCubic().setOnComplete(() =>
            {
                adventureButton.interactable = true;
                
                cooldownBar.fillAmount = 0f;
            });
        }
        private void GenerateArena()
        {
            activeRoutine = StartCoroutine(UIManager.BruteForceTypeWriterRoutine(adventureText, "Enemy has appeared."));
            combatPopup.SetActive(true);
            combatPopup.transform.localScale = Vector3.zero;
            LeanTween.scale(combatPopup, Vector3.one, 0.5f).setEaseOutBack();

        }
        private void GenerateAdventureText()
        {
            string activeLocation = GameManager.Instance.Context.Player.ActiveLocation;
            int maxTextCount = GameManager.Instance.Context.Locations.Find(l => l.ID == activeLocation)?.AdventureTexts.Count ?? 0;
            string randomText = GameManager.Instance.Context.Locations.Find(l => l.ID == activeLocation).AdventureTexts[Random.Range(0, maxTextCount)];
            
             activeRoutine = StartCoroutine(UIManager.BruteForceTypeWriterRoutine(adventureText, randomText));
        }
        
    }
}
