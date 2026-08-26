using Assets._Project.Scripts.Managers.AdventureSystem;
using Assets._Project.Scripts.UI;
using Assets._Project.Scripts.UI.Cards;
using System;
using System.Collections;
using TextBasedRPG.Core.Entities;
using TextBasedRPG.Core.Heroes;
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
        //[SerializeField] Image wobbleImage;
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
        AudioManager audioManager => AudioManager.Instance;
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

            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"<color=#3E9BD9>{generatedEnemy.Name}</color> appears!"));
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


        #endregion

        #region Loot System
        private void GiveLoot(GameContext context, Entity enemy)
        {
            RewardGoldAndExp(context, enemy);
            RewardItem(context, enemy);
        }

        private static void RewardItem(GameContext context, Entity enemy)
        {
            LootResult ls = LootManager.EnemyLootGenerator(enemy);
            
            var itemTemplate = LootManager.FindItemByID(ls.ID);

            if (itemTemplate != null)
            {
                InventoryManager.AddToInventory(ls);
                //Toaster.Instance.ShowToast($"You found {amount}x {itemTemplate.Name}!", UIManager.Instance.IconDB.confirmIcon);
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

            Toaster.Instance.ShowToast($"You got {finalGold} gold and {finalExp} EXP!", UIManager.Instance.IconDB.confirmIcon);
        }
        #endregion

        #region Event Handlers
        private void OnPlayerGotHit(bool isCrit, int damage)
        {
            RightSectionManager rsm = UIManager.Instance.rightSection.gameObject.GetComponent<RightSectionManager>();
            RectTransform playerRect = rsm.playerPanel.playerAvatar.gameObject.GetComponent<RectTransform>();
                
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

            UIExtensions.Flash(rsm.playerPanel.playerAvatar);
            UIExtensions.GenerateDamageText(rsm.playerPanel.damageText, isCrit, damage);
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
            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"<color=#FFFFFF>{generatedEnemy.Name}</color> is preparing to attack..."));

            yield return new WaitForSeconds(2.5f);

            float isHit = UnityEngine.Random.Range(0, 101);

            if (isHit <= 95)
            {
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
                string critText = isCrit ? " <color=#C9622E>Critical hit!</color>" : "";

                StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText,
                    $"<color=#FFFFFF>{generatedEnemy.Name}</color> dealt {calculatedDamage} damage! {critText}"));

                EventManager.CombatEvents.TriggerOnPlayerGotHit(isCrit, calculatedDamage);
                EventManager.HeroEvents.TriggerHPValueChanged(GameManager.Instance.Context);

                float randomPitch = UnityEngine.Random.Range(0.95f, 1.05f);

                int audioListCount = audioManager.audioDB.gettingHit.Count;
                AudioManager.Instance.PlaySFX(audioManager.audioDB.gettingHit[UnityEngine.Random.Range(0, audioListCount)], randomPitch);
            } else
            {
                StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText,
                    $"<color=#FFFFFF>{generatedEnemy.Name}</color> couldn't land the attack."));
            }
            

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
            string red = "#A6293A";
            string yellow = "#B8D93E";
            string green = "#4FBF7A";

            float hpPercentage = (float)enemy.CurHP / enemy.TotalHP;

            string coloredName = $"<color={red}>{enemy.Name}</color>";

            if (hpPercentage <= 1f)
            {
                coloredName = $"<color={green}>{enemy.Name}</color>";
                return $"{coloredName} looks ready to fight!";
            }
            if (hpPercentage <= 0.66f)
            {
                coloredName = $"<color={yellow}>{enemy.Name}</color>";
                return $"{coloredName} is watching you carefully.";
            }
            if (hpPercentage <= 0.33f) {
                coloredName = $"<color={red}>{enemy.Name}</color>";
                return $"{coloredName} seems tired to fight...";
            }
            if (hpPercentage <= 0f)
            {
                coloredName = $"<color={red}>{enemy.Name}</color>";
                return $"{coloredName} is fainted!";
            }
            return $"{coloredName} is fainted!";
        }
        
        #endregion
    }
}