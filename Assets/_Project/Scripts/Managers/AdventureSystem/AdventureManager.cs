using System;
using TMPro;
using TextBasedRPG.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.Managers.Adventure
{
    public class AdventureManager : MonoBehaviour
    {
        public static AdventureManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] TMP_Text adventureText;
        [SerializeField] TMP_Text adventureBoost;
        [SerializeField] Button adventureButton;
        [SerializeField] Image cooldownBar;
        [SerializeField] GameObject combatPopup;
        [SerializeField] GameObject itemPopup;
        [SerializeField] int cooldown;

        [Header("Chances")]
        [Range(0, 100)] public int adventureTextChance = 60;
        [Range(0, 100)] public int enemyEncounterChance = 30;
        [Range(0, 100)] public int itemDropChance = 10;

        [Header("Boost Settings")]
        public float adventureBoostInit = 1f;
        public float adventureBoostMultiplier = 0.05f;

        public float CurrentBoost { get; private set; }

        Coroutine activeRoutine;

        private void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            EventManager.CombatEvents.OnPlayerDied += ResetBoost;
            if (CurrentBoost == 0f) CurrentBoost = adventureBoostInit;
            UpdateBoostText();
        }

        private void OnDisable()
        {
            EventManager.CombatEvents.OnPlayerDied -= ResetBoost;
            ResetBoost();
        }

        public void OnAdventureButtonClicked()
        {
            ManageButtonCooldown();
            ProcessRandomEvent();
            IncreaseBoost();
        }

        private void IncreaseBoost()
        {
            CurrentBoost += adventureBoostMultiplier;
            UpdateBoostText();
        }

        public void ResetBoost()
        {
            CurrentBoost = adventureBoostInit;
            UpdateBoostText();
        }

        private void UpdateBoostText()
        {
            if (adventureBoost != null)
                adventureBoost.text = $"Adventure Boost: x{CurrentBoost:F2}";
        }

        private void ProcessRandomEvent()
        {
            if (activeRoutine != null) StopCoroutine(activeRoutine);
            activeRoutine = null;

            int totalChance = itemDropChance + enemyEncounterChance + adventureTextChance;
            int roll = UnityEngine.Random.Range(0, totalChance);

            if (roll < itemDropChance)
                GenerateItem();
            else if (roll < itemDropChance + enemyEncounterChance)
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
            string randomText = GameManager.Instance.Context.Locations.Find(l => l.ID == activeLocation).AdventureTexts[UnityEngine.Random.Range(0, maxTextCount)];

            activeRoutine = StartCoroutine(UIManager.BruteForceTypeWriterRoutine(adventureText, randomText));

            RewardAdventureText();
        }

        private void RewardAdventureText()
        {
            var player = GameManager.Instance.Context.Player;

            float expBase = 10f * MathF.Pow(player.Level, 1.3f);
            int finalExp = (int)Math.Round(expBase * CurrentBoost);
            player.CurExp += finalExp;
            player.TotalExp += finalExp;
            EventManager.HeroEvents.TriggerExpChanged(GameManager.Instance.Context);

            float goldBase = 5f * MathF.Sqrt(player.Level);
            int finalGold = (int)Math.Round(goldBase * CurrentBoost);
            player.Gold += finalGold;
            EventManager.HeroEvents.TriggerGoldChanged(GameManager.Instance.Context);

            Toaster.Instance.ShowToast($"Explored! +{finalGold} gold, +{finalExp} EXP", UIManager.Instance.IconDB.confirmIcon);
        }

        private void GenerateItem()
        {
            activeRoutine = StartCoroutine(UIManager.BruteForceTypeWriterRoutine(adventureText, "You found an item!"));
            itemPopup.SetActive(true);
        }
    }
}
