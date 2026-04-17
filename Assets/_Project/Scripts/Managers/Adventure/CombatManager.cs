using Assets._Project.Scripts.UI;
using Assets._Project.Scripts.UI.Cards;
using System;
using System.Collections;
using System.Linq;
using TextBasedRPG.Core.Entities;
using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Events;
using TextBasedRPG.Managers.DataManagement;
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
        #region Variables
        [Header("Enemy UI References")]
        [SerializeField] TMP_Text entityName;
        [SerializeField] public Image entitySprite;
        [SerializeField] TMP_Text level;
        [SerializeField] TMP_Text attack;
        [SerializeField] TMP_Text def;
        [SerializeField] TMP_Text speed;
        [SerializeField] TMP_Text hp;
        [SerializeField] Image ghostFill;
        [SerializeField] Image healthSprite;
        [SerializeField] public TMP_Text damageText;

        [Header("Other")]
        [SerializeField] Image wobbleImage;
        [SerializeField] Image vignette;
        [SerializeField] TMP_Text contentText;
        [SerializeField] public bool isPlayerTurn;
        [SerializeField] public bool isCombatActive = false;

        [Header("Shake Configs")]
        [SerializeField] public float shakeIntensity = 3f;
        [SerializeField] public float shakeDuration = 0.25f;

        [Header("Focus And Guard")]
        // Can't miss attack while focused. Focus resets if you hit.
        // Each focus stack gives you +5% crit chance, +10% crit damage and +5% attack.
        public int focusAmount = 0;
        public int focusCap = 4;
        // Guard gives stackable hp along the halving taken damage. Can't be used back to back
        public int guardAmount = 0;
        public int guardPerMove = 25; // %
        public int maxShield = 100;
        public bool didGuardLastTurn = false;

        public Entity generatedEnemy;
        CombatActions ca;
        #endregion
        private void Start()
        {
            focusCap = 4;
            guardPerMove = (int)(GameManager.Instance.Context.Player.TotalHP * 0.10f); // %
            maxShield = (int)(GameManager.Instance.Context.Player.TotalHP * 0.25f);
        }
        #region OnEnable & OnDisable
        private void OnEnable()
        {
            StartCoroutine(SetupCombatRoutine());

            ca = gameObject.GetComponent<CombatActions>();
            EventManager.CombatEvents.OnPlayerGotHit += OnPlayerGotHit;
            EventManager.CombatEvents.OnPlayerLowHP += OnPlayerLowHP;
        }
        
        private void OnDisable()
        {
            LeanTween.cancel(entitySprite.gameObject);

            EventManager.CombatEvents.OnPlayerGotHit -= OnPlayerGotHit;
            EventManager.CombatEvents.OnPlayerLowHP -= OnPlayerLowHP;
        }

        private IEnumerator SetupCombatRoutine()
        {
            // Waiting 1 frame so that Context and Entities are properly initialized in GameManager
            yield return null;

            InitializeArena();
            StartCombat();

            LeanTween.cancel(entitySprite.gameObject);
            LeanTween.scale(entitySprite.gameObject, Vector3.one * 1.05f, 2f)
                .setLoopPingPong()
                .setEaseInOutCubic();

        }
        #endregion

        #region Initialize & Start Combat & End Combat
        private void InitializeArena()
        {
            if (GameManager.Instance == null) { Debug.LogError("GameManager.Instance bulunamadı!"); return; }
            if (GameManager.Instance.Context == null) { Debug.LogError("Context henüz oluşturulmamış (null)!"); return; }
            if (GameManager.Instance.Context.Entities == null) { Debug.LogError("Entities listesi null!"); return; }

            generatedEnemy = null;
            generatedEnemy = EnemyGenerator.GenerateEnemy(GameManager.Instance.Context);
            string path = "EntitySprites/" + generatedEnemy.EntitySprite;

            entityName.text = generatedEnemy.Name;
            entitySprite.sprite = Resources.Load<Sprite>(path);
            level.text = $"Lv - {generatedEnemy.GeneratedLevel}";
            attack.text = $"{generatedEnemy.TotalATK}";
            def.text = $"{generatedEnemy.TotalDEF}";
            speed.text = $"{generatedEnemy.CurrentSPD}";
            hp.text = $"{(float)generatedEnemy.CurHP} / {generatedEnemy.TotalHP}";
            UpdateHealthUI(true);
            contentText.text = string.Empty;

            focusAmount = 0;
            guardAmount = 0;
            ca.OnFocusChanged();
            ca.OnGuardChanged();

            ca.fleeBtn.GetComponentInChildren<TMP_Text>().text =
                $"Run Away - {CombatActions.CalculateRunAwayChance(GameManager.Instance.Context, generatedEnemy).ToString()}%";

            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"<color=#0F172A>{generatedEnemy.Name}</color> appears!"));
        }
        private void StartCombat()
        {
            isCombatActive = true;
            //StartWobble();
            isPlayerTurn = GameManager.Instance.Context.Player.TotalSPD >= generatedEnemy.CurrentSPD;


            if (!isPlayerTurn)
            {
                SetButtonsInteractable(false);
                StartCoroutine(EnemyTurnRoutine());
            }
            else
            {
                SetButtonsInteractable(true);
                ca.fleeBtn.GetComponentInChildren<TMP_Text>().text = $"Run Away - {CombatActions.CalculateRunAwayChance(GameManager.Instance.Context, generatedEnemy).ToString()}%";
                contentText.text += "";
                StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"Your turn! What will you do?"));

            }
        }
        
        public void EndCombat(CombatResult result)
        {
            isCombatActive = false;
            SetButtonsInteractable(false);
            StartWobble();

            if (result == CombatResult.Victory)
            {
                StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, "You crushed the enemy."));
                GiveLoot(GameManager.Instance.Context, generatedEnemy);
            }
            else if (result == CombatResult.Defeat)
            {
                StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, "You have Died! You will be penalized."));
                StartCoroutine(WaitToPenalize(2f));
            } else if (result == CombatResult.RunAway)
            {
                StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, "Your journey continues..."));
                
            }
            GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);
            StartCoroutine(ClosePopup());

            
        }

        private void DisableNavbar()
        {

        }
        private IEnumerator ClosePopup()
        {
            yield return new WaitForSeconds(2f);
            LeanTween.scale(gameObject, Vector3.zero, 0.5f).setEaseOutBack();
            yield return new WaitForSeconds(0.5f);
            gameObject.SetActive(false);
        }

        public void StartWobble()
        {
            if (isCombatActive)
            {
                LeanTween.rotateAroundLocal(wobbleImage.gameObject, Vector3.forward, 2f, 2.5f)
                .setEaseInOutSine()
                .setLoopPingPong();

                LeanTween.scale(wobbleImage.gameObject, new Vector3(1.05f, 1.05f, 1f), 3f)
                    .setEaseInOutSine()
                    .setLoopPingPong();
            }
            else
            {
                LeanTween.cancel(wobbleImage.gameObject);
            }
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
            float randomMultiplier = 1 + (UnityEngine.Random.Range(0.0f, 1.0f) * (0.15f));
            int finalGold = (int)Math.Round(goldBase * randomMultiplier);
            context.Player.Gold += finalGold;
            EventManager.HeroEvents.TriggerGoldChanged(context);

            float levelDiffBonus = (enemy.GeneratedLevel > context.Player.Level) ? 1.2f : (enemy.GeneratedLevel < context.Player.Level ? 0.8f : 1.0f);
            float expBase = (enemy.PowerScore * enemy.GeneratedLevel) / 5.0f;
            int finalExp = (int)Math.Round(expBase * levelDiffBonus);
            context.Player.CurExp += finalExp;
            context.Player.TotalExp += finalExp;
            EventManager.HeroEvents.TriggerExpChanged(context);
        }
        #endregion

        #region Event Handlers
        private void OnPlayerGotHit(bool isCrit, int damage)
        {
            RightSectionManager rsm = UIManager.Instance.rightSection.gameObject.GetComponent<RightSectionManager>();
            RectTransform playerRect = rsm.playerAvatar.gameObject.GetComponent<RectTransform>();
                
            if (isCrit)
            {
                UIExtensions.Shake(playerRect, shakeIntensity + 7f, shakeDuration*2);
                UIExtensions.Shake(gameObject.GetComponent<RectTransform>(), shakeIntensity + 7f, shakeDuration * 2);
            }
            else
            {
                UIExtensions.Shake(playerRect, shakeIntensity, shakeDuration);
                UIExtensions.Shake(gameObject.GetComponent<RectTransform>(), shakeIntensity, shakeDuration);
            }

            UIExtensions.Flash(rsm.playerAvatar);
            UIExtensions.GenerateDamageText(rsm.damageText, isCrit, damage);
        }
        public void OnPlayerLowHP()
        {
            Hero player = GameManager.Instance.Context.Player;
            if (vignette.gameObject != null)
            {
                if (player.CurHP >= player.TotalHP * 0.20f)
                {
                    LeanTween.cancel(vignette.gameObject);
                    vignette.color = new Color(vignette.color.r, vignette.color.g, vignette.color.b, 0f);
                }
                else
                {
                    LeanTween.value(vignette.gameObject, 64f / 255f, 128f / 255f, 0.5f)
                        .setEaseInOutCubic()
                        .setOnUpdate((float val) => vignette.color = new Color(vignette.color.r, vignette.color.g, vignette.color.b, val)).
                        setLoopPingPong();
                }
            } 
        }
        #endregion

        #region Enemy Actions
        public IEnumerator EnemyTurnRoutine()
        {
            Hero player = GameManager.Instance.Context.Player;

            contentText.text = string.Empty;
            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"<color=#0F172A>{generatedEnemy.Name}</color> is preparing to attack..."));

            yield return new WaitForSeconds(2.5f);

            IDamageCalculator damage = new DamageCalculator();
            int calculatedDamage = damage.CalculateDMG(
                generatedEnemy.TotalATK,
                player.TotalDEF,
                0f,
                0f,
                out bool isCrit);

            if (didGuardLastTurn)
            {
                calculatedDamage = Mathf.CeilToInt(calculatedDamage * 0.5f);
            }
            int damageRemaining = calculatedDamage;

            if (guardAmount > 0)
            {
                if (guardAmount >= damageRemaining)
                {
                    guardAmount -= damageRemaining;
                    damageRemaining = 0;
                }
                else
                {
                    damageRemaining -= guardAmount;
                    guardAmount = 0;
                }

                EventManager.CombatEvents.TriggerOnGuardChanged();
            }

            if (damageRemaining > 0)
            {
                if (damageRemaining >= player.CurHP)
                {
                    damageRemaining = player.CurHP;
                    player.CurHP = 0;
                    player.Deaths += 1;
                }
                else
                {
                    player.CurHP -= damageRemaining;
                }
            }

            didGuardLastTurn = false;
            string critText = isCrit ? " <color=#0F172A>Critical hit!</color>" : "";

            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText,
                $"<color=#0F172A>{generatedEnemy.Name}</color> dealt {calculatedDamage} damage! {critText}"));

            EventManager.CombatEvents.TriggerOnPlayerGotHit(isCrit, calculatedDamage);
            EventManager.HeroEvents.TriggerHPValueChanged(GameManager.Instance.Context);

            yield return new WaitForSeconds(2.5f);

            if (player.CurHP <= 0)
            {
                EndCombat(CombatResult.Defeat);
            }
            else
            {
                isPlayerTurn = true;
                SetButtonsInteractable(true);
                StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, "Your turn!"));
            }
        }
        #endregion

        #region mix
        IEnumerator WaitToPenalize(float delay)
        {
            yield return new WaitForSeconds(delay);
            GameManager.Instance.Context.Player.ApplyDeathPenalty();
        }
        public void UpdateHealthUI(bool instant)
        {
            float targetFill = (float)generatedEnemy.CurHP / generatedEnemy.TotalHP;
            hp.text = $"{generatedEnemy.CurHP} / {generatedEnemy.TotalHP}";

            if (instant)
            {
                healthSprite.fillAmount = targetFill;
                ghostFill.fillAmount = targetFill;
            }
            else
                UIExtensions.GhostBarFill(healthSprite, ghostFill, targetFill);
        }
        public void SetButtonsInteractable(bool state)
        {
            CombatActions ca = gameObject.GetComponent<CombatActions>();
            ca.attackBtn.interactable = state;
            ca.focusBtn.interactable = state;
            ca.guardBtn.interactable = state;
            ca.backpackBtn.interactable = state;
            ca.fleeBtn.interactable = state;
            Canvas.ForceUpdateCanvases();
        }
        public string GetEnemyStatus(Entity enemy)
        {
            string red = "#8B0000";
            string yellow = "#8C7014";
            string green = "#17761E";

            float hpPercentage = (float)enemy.CurHP / enemy.TotalHP;

            if (hpPercentage >= 0.60f)
            {
                string coloredName = $"<color={green}>{enemy.Name}</color>";
                return $"{coloredName} looks ready to fight!";
            }
            if (hpPercentage >= 0.40f) {
                string coloredName = $"<color={yellow}>{enemy.Name}</color>";
                return $"{coloredName} is watching you carefully";
            }
            if (hpPercentage >= 0.20f)
            {
                string coloredName = $"<color={red}>{enemy.Name}</color>";
                return $"{coloredName} looks tired to fight...";
            }
            string panicName = $"<color={red}>{enemy.Name}</color>";
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
        #endregion
    }
}