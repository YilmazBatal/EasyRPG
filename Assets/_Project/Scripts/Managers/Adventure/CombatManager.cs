using System;
using System.Collections;
using System.Linq;
using TextBasedRPG.Core.Entities;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TextBasedRPG.Managers
{
    /// <summary>
    /// This is attached to compat panel pop up
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        [Header("Enemy UI References")]
        [SerializeField] TMP_Text entityName;
        [SerializeField] Image entitySprite;
        [SerializeField] TMP_Text level;
        [SerializeField] TMP_Text attack;
        [SerializeField] TMP_Text def;
        [SerializeField] TMP_Text speed;
        [SerializeField] TMP_Text hp;
        [SerializeField] Image healthSprite;

        [Header("Other")]
        [SerializeField] TMP_Text contentText;
        [SerializeField] public bool isPlayerTurn;
        [SerializeField] public bool isCombatActive = false;

        public Entity generatedEnemy;


        private void OnEnable()
        {
            InitializeArena();
            StartCombat();
        }
        #region Initialize & Start Combat & End Combat
        private void InitializeArena()
        {
            generatedEnemy = EnemyGenerator.GenerateEnemy(GameManager.Instance.Context);
            string path = "EntitySprites/" + generatedEnemy.EntitySprite;
            Debug.Log(path);

            entityName.text = generatedEnemy.Name;
            entitySprite.sprite = Resources.Load<Sprite>(path);
            level.text = $"{generatedEnemy.GeneratedLevel}";
            attack.text = $"{generatedEnemy.TotalATK}";
            def.text = $"{generatedEnemy.TotalDEF}";
            speed.text = $"{generatedEnemy.CurrentSPD}";
            hp.text = $"{(float)generatedEnemy.CurHP} / {generatedEnemy.TotalHP}";
            UpdateHealthUI(true);
            contentText.text = string.Empty;

            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"<color=#0F172A>{generatedEnemy.Name}</color> appears!"));
        }
        private void StartCombat()
        {
            isCombatActive = true;

            isPlayerTurn = GameManager.Instance.Context.Player.TotalSPD >= generatedEnemy.CurrentSPD;

            if (!isPlayerTurn)
            {
                SetButtonsInteractable(false);
                StartCoroutine(EnemyTurnRoutine());
            }
            else
            {
                SetButtonsInteractable(true);
                contentText.text += "";
                StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"Your turn! What will you do?"));
            }
        }
        
        public void EndCombat(bool victory)
        {
            isCombatActive = false;
            SetButtonsInteractable(false);

            if (victory)
            {
                StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, "Victory! You defeated the enemy."));
                GiveLoot(GameManager.Instance.Context, generatedEnemy);
            }
            else
            {
                StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, "You have Died! You will be penalized."));
                GameManager.Instance.Context.Player.ApplyDeathPenalty();
            }
            StartCoroutine(ClosePopup());
        }

        private IEnumerator ClosePopup()
        {
            yield return new WaitForSeconds(2f);
            LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEaseOutBack();
            gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
        #endregion
        
        #region Loot System
        private void GiveLoot(GameContext context, Entity enemy)
        {
            RewardGoldAndExp(context, enemy);
            RewardItem(context);
        }

        private static void RewardItem(GameContext context)
        {
            var currentMap = context.Locations.FirstOrDefault(l => l.ID == context.Player.ActiveLocation);

            if (currentMap?.AdventureLoots != null && currentMap.AdventureLoots.Count > 0)
            {
                int totalWeight = currentMap.AdventureLoots.Sum(x => x.DropChance);
                int roll = UnityEngine.Random.Range(0, totalWeight);
                int currentWeight = 0;

                foreach (var loot in currentMap.AdventureLoots)
                {
                    currentWeight += loot.DropChance;

                    if (roll < currentWeight)
                    {
                        int amount = UnityEngine.Random.Range(1, loot.MaxAmount + 1);

                        var itemTemplate = FindItemByID(context, loot.ID);

                        if (itemTemplate != null)
                        {
                            InventoryManager.AddToInventory(context, loot, amount);
                            //MenuUI.ColoredMsg(ConsoleColor.Green, $"You found {amount}x {itemTemplate.Name}!");
                        }

                        break;
                    }
                }
            }
        }

        private static void RewardGoldAndExp(GameContext context, Entity enemy)
        {

            float goldBase = enemy.PowerScore * enemy.GoldMultiplier * MathF.Sqrt(enemy.GeneratedLevel);
            Debug.Log("enemy.PowerScore: " + enemy.PowerScore);
            Debug.Log("Enemy Gold Multiplier: " + enemy.GoldMultiplier);
            Debug.Log("MathF.Sqrt(enemy.Level): " + MathF.Sqrt(enemy.GeneratedLevel));
            Debug.Log("(enemy.Level): " + (enemy.GeneratedLevel));
            Debug.Log("Gold Base: " + goldBase);

            float randomMultiplier = 1 + (UnityEngine.Random.Range(0.0f, 1.0f) * (0.15f));
            int finalGold = (int)Math.Round(goldBase * randomMultiplier);
            context.Player.Gold += finalGold;

            Debug.Log("New amount : " + context.Player.Gold);
            EventManager.HeroEvents.TriggerGoldChanged(context);

            float levelDiffBonus = (enemy.Level > context.Player.Level) ? 1.2f : (enemy.Level < context.Player.Level ? 0.8f : 1.0f);
            float expBase = (enemy.PowerScore * enemy.Level) / 5.0f;
            int finalExp = (int)Math.Round(expBase * levelDiffBonus);
            context.Player.CurExp += finalExp;
            EventManager.HeroEvents.TriggerExpGained(context.Player.CurExp);
        }
        #endregion

        public IEnumerator EnemyTurnRoutine()
        {
            contentText.text = string.Empty;;
            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"<color=#0F172A>{generatedEnemy.Name}</color> is preparing to attack..."));

            yield return new WaitForSeconds(2.5f);

            int damage = Mathf.Max(1, generatedEnemy.TotalATK - GameManager.Instance.Context.Player.TotalDEF);
            GameManager.Instance.Context.Player.CurHP -= damage;

            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"<color=#0F172A>{generatedEnemy.Name}</color> dealt {damage} damage to you!"));

            yield return new WaitForSeconds(2.5f);

            if (GameManager.Instance.Context.Player.CurHP <= 0)
            {
                EndCombat(false);
            }
            else
            {
                isPlayerTurn = true;
                SetButtonsInteractable(true);
                StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, "Your turn!"));
            }
        }


        public void UpdateHealthUI(bool instant)
        {
            float targetFill = (float)generatedEnemy.CurHP / generatedEnemy.TotalHP;
            hp.text = $"{generatedEnemy.CurHP} / {generatedEnemy.TotalHP}";

            if (instant)
                healthSprite.fillAmount = targetFill;
            else
            {
                LeanTween.value(healthSprite.gameObject, healthSprite.fillAmount, targetFill, 0.5f)
                    .setEase(LeanTweenType.easeInOutQuad) 
                    .setOnUpdate((float val) =>
                    {
                        healthSprite.fillAmount = val;
                    });
            }
        }

        public void SetButtonsInteractable(bool state)
        {
            CombatActions ca = gameObject.GetComponent<CombatActions>();
            ca.attackBtn.interactable = state;
            ca.focusBtn.interactable = state;
            ca.guardBtn.interactable = state;
            ca.backpackBtn.interactable = state;
            ca.fleeBtn.interactable = state;
        }

        public string GetEnemyStatus(Entity enemy)
        {
            string red = "#FF4C4C"; 
            string yellow = "#FFD700"; 

            string coloredName = $"<color={red}>{enemy.Name}</color>";
            float hpPercentage = (float)enemy.CurHP / enemy.TotalHP;

            if (hpPercentage >= 0.75f)
                return $"{coloredName} looks ready to fight!";

            if (hpPercentage >= 0.50f)
                return $"{coloredName} is watching you carefully";

            if (hpPercentage >= 0.20f)
                return $"{coloredName} looks slightly tired to fight...";

            string panicName = $"<color={yellow}>{enemy.Name}</color>";
            return $"{panicName} is panicking!";
        }

        private static Item? FindItemByID(GameContext context, string id)
        {
            if (id.StartsWith("W")) return context.Weapons?.FirstOrDefault(i => i.ID == id);
            if (id.StartsWith("A")) return context.Armors?.FirstOrDefault(i => i.ID == id);
            if (id.StartsWith("M")) return context.Materials?.FirstOrDefault(i => i.ID == id);
            if (id.StartsWith("C")) return context.Consumables?.FirstOrDefault(i => i.ID == id);

            return null;
        }
    }
}