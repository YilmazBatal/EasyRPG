using Assets._Project.Scripts.UI;
using Assets._Project.Scripts.UI.Cards;
using System;
using System.Collections;
using TextBasedRPG.Core.Entities;
using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Events;
using TextBasedRPG.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CombatActions : MonoBehaviour
{
    #region References
    [Header("CM & Buttons")]
    [SerializeField] CombatManager combatManager;
    [SerializeField] TMP_Text contentText;
    [SerializeField] public Button attackBtn;
    [SerializeField] public Button focusBtn;
    [SerializeField] public Button guardBtn;
    [SerializeField] public Button backpackBtn;
    [SerializeField] public Button fleeBtn;
    [Header("Focus and Guard")]
    [SerializeField] public Image focusBar;
    [SerializeField] public Image focusBarGhost;
    [SerializeField] public Image guardBar;
    [SerializeField] public Image guardBarGhost;

    // Move to datamanager - static data later
    string[] focusMessages = {
        "You guys started to make eye contact.",
        "Inner peace found. Inner violence... loading.",
        "You narrowed your eyes so much that you can barely see the enemy now. But hey, you're focused!",
        "You're doing complex math in your head. 2+2 is... 4! Quick, attack before you forget!",
        "You're staring at the enemy so intensely that it's starting to feel awkward for both of you."
    };

    GameContext context;
    RightSectionManager rsm;
    #endregion

    #region Enable & Disable
    private void Start() {
        context = GameManager.Instance.Context;
        rsm = GameObject.Find("RightStatusPanel").GetComponent<RightSectionManager>();
    }
    private void OnEnable()
    {
        EventManager.CombatEvents.OnEntityGotHit += OnEntityGotHit;
        EventManager.CombatEvents.OnGuardChanged += OnGuardChanged;
        EventManager.CombatEvents.OnFocusChanged += OnFocusChanged;
    }
    private void OnDisable()
    {
        EventManager.CombatEvents.OnEntityGotHit -= OnEntityGotHit;
        EventManager.CombatEvents.OnGuardChanged -= OnGuardChanged;
        EventManager.CombatEvents.OnFocusChanged -= OnFocusChanged;
    }
    #endregion

    #region Attack
    public void OnAttackClicked()
    {
        if (!combatManager.isPlayerTurn) return;
        StartCoroutine(PlayerAttackRoutine());
    }

    private IEnumerator PlayerAttackRoutine()
    {
        Hero p = context.Player;
        combatManager.SetButtonsInteractable(false);


        float hitAccuracy = Random.value;

        if (combatManager.focusAmount > 0 || hitAccuracy <= 0.95f)
        {
            combatManager.focusAmount = 0;
            p.BonusATK = 0;
            p.BonusCritRate = 0;
            p.BonusCritDMG = 0;
            UIExtensions.GhostBarFill(focusBar, focusBarGhost, combatManager.focusAmount);
            rsm.quickStatsPanel.PlayerQuickStats(context);

            IDamageCalculator damage = new DamageCalculator();
            int calculatedDamage = damage.CalculateDMG(
                context.Player.TotalATK,
                combatManager.generatedEnemy.TotalDEF,
                context.Player.CritRate,
                context.Player.CritDamage,
                out bool isCrit);

            p.UpdateHeaviestDamage(calculatedDamage);

            if (calculatedDamage >= combatManager.generatedEnemy.CurHP)
            {
                //calculatedDamage = combatManager.generatedEnemy.CurHP;
                combatManager.generatedEnemy.CurHP = 0;
                p.EntitiesSlayed += 1;
            }
            else
                combatManager.generatedEnemy.CurHP -= calculatedDamage;

            string typeID = combatManager.generatedEnemy.EntityTypeID;
            Debug.Log($"[COMBAT] Audio request sent. ID: '{typeID}'");

            if (AudioManager.Instance == null)
            {
                Debug.LogError("[COMBAT] AudioManager Instance NULL!");
            }
            else
            {
                AudioManager.Instance.PlayHitSound(typeID);
            }

            string critText = isCrit ? "<color=#0F172A>Critical hit!</color>" : "";
            bool wasFocused = false;
            if (combatManager.focusAmount > 0)
                wasFocused = true;
            else
                wasFocused = false;

            EventManager.CombatEvents.TriggerOnEntityGotHit(isCrit, calculatedDamage, wasFocused);

            contentText.text = string.Empty;

            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, combatManager.GetEnemyStatus(combatManager.generatedEnemy)));

            yield return new WaitForSeconds(2f);

            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(
                contentText,
                $"You dealt <color=#A6293A>{calculatedDamage}</color> to {combatManager.generatedEnemy.Name}! {critText}"));

            yield return new WaitForSeconds(2f);

            if (combatManager.generatedEnemy.CurHP <= 0)
            {
                combatManager.EndCombat(CombatResult.Victory);
            }
            else
            {
                combatManager.isPlayerTurn = false;
                StartCoroutine(combatManager.EnemyTurnRoutine());
            }
            //AudioManager.Instance.PlaySFX(AudioManager.Instance.audioDB.[wolfHit[random]], 1f);
        }
        else if (hitAccuracy > 0.95f)
        {
            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"{combatManager.generatedEnemy.Name} somehow dodged this."));
            yield return new WaitForSeconds(2f);
            combatManager.isPlayerTurn = false;
            StartCoroutine(combatManager.EnemyTurnRoutine());
        }
    }
    #endregion

    #region Focus
    public void OnFocusClicked()
    {
        if (!combatManager.isPlayerTurn) return;
        StartCoroutine(PlayerFocusRoutine());
    }

    private IEnumerator PlayerFocusRoutine()
    {
        Hero p = context.Player;
        combatManager.SetButtonsInteractable(false);

        combatManager.focusAmount = Mathf.Min(combatManager.focusAmount + 1, combatManager.focusCap);

        p.BonusATK = (int)(p.BonuslessATK * 0.10 * combatManager.focusAmount);
        p.BonusCritRate = (5 * combatManager.focusAmount);
        p.BonusCritDMG = (10 * combatManager.focusAmount);

        EventManager.CombatEvents.TriggerOnFocusChanged();

        contentText.text = string.Empty;
        string randomMsg = focusMessages[Random.Range(0, focusMessages.Length)];
        StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, randomMsg));
        yield return new WaitForSeconds(3f);

        // if posion or something that damages player at the end of turn
        // add if section
        StartCoroutine(combatManager.EnemyTurnRoutine());
    }
    #endregion

    #region Guard Up
    public void OnGuardUpClicked()
    {
        if (!combatManager.isPlayerTurn) return;
        StartCoroutine(PlayerGuardUpRoutine());
    }

    private IEnumerator PlayerGuardUpRoutine()
    {
        Hero p = context.Player;
        combatManager.SetButtonsInteractable(false);

        if (combatManager.didGuardLastTurn)
        {
            yield return StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, "You can't spam that silly. You aren't gonna be immortal."));
            yield return new WaitForSeconds(3f);
            combatManager.SetButtonsInteractable(true);
            yield break;
        }

        int gain = (int)(context.Player.TotalHP * 0.10f);
        combatManager.guardAmount = Mathf.Min(combatManager.guardAmount + gain, combatManager.maxShield);
        combatManager.didGuardLastTurn = true;

        EventManager.CombatEvents.TriggerOnGuardChanged();

        yield return StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, "You prepared your shield!"));
        yield return new WaitForSeconds(2f);

        combatManager.isPlayerTurn = false;

        // if posion or something that damages player at the end of turn
        // add if section
        StartCoroutine(combatManager.EnemyTurnRoutine());
    }
    #endregion

    #region Backpack
    public void OnBackpackClicked()
    {
        if (!combatManager.isPlayerTurn) return;
        StartCoroutine(PlayerBackpackRoutine());
    }

    private IEnumerator PlayerBackpackRoutine()
    {
        Hero p = context.Player;
        combatManager.SetButtonsInteractable(false);

        yield return StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, "Not implemented yet."));
        yield return new WaitForSeconds(3f);
        combatManager.SetButtonsInteractable(true);
        yield break;
    }
    #endregion

    #region Run Away
    public void OnFleeClicked()
    {
        if (!combatManager.isPlayerTurn) return;
        StartCoroutine(PlayerFleeRoutine());
    }

    private IEnumerator PlayerFleeRoutine()
    {
        combatManager.SetButtonsInteractable(false);

        int roll = Random.Range(0, 101);
        bool success = roll < CalculateRunAwayChance(context, combatManager.generatedEnemy);

        if (success)
        {
            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"You were so fast that your feets touched your cheeks. You've run away from the <color=#0F172A>{combatManager.generatedEnemy.Name}</color> <color=#17761E>successfuly</color>."));
            yield return new WaitForSeconds(3f);
            
            combatManager.EndCombat(CombatResult.RunAway);
        }
        else
        {
            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"That thing is so fast.  You <color=#8B0000>couldn't</color> escape from the <color=#0F172A>{combatManager.generatedEnemy.Name}</color>."));
            yield return new WaitForSeconds(3f);
            
            combatManager.isPlayerTurn = false;
            StartCoroutine(combatManager.EnemyTurnRoutine());
        }
    }

    public static int CalculateRunAwayChance(GameContext context, Entity entity)
    {
        int enemySpeed = entity.CurrentSPD;
        int playerSpeed = context.Player.TotalSPD;
        int baseLuck = 50; // %
        int luckMultiplier = 2;
        int runAwayChance = baseLuck + (playerSpeed - enemySpeed) * luckMultiplier;
        int runAwayChanceFinal = Math.Clamp(runAwayChance, 0, 100);
        return runAwayChanceFinal;
    }

    #endregion

    #region Events
    private void OnEntityGotHit(bool isCrit, int damage, bool wasFocused)
    {
        if (wasFocused)
        {
            UIExtensions.Shake(combatManager.entitySprite.rectTransform, combatManager.shakeIntensity + 20f, combatManager.shakeDuration * 3);
            UIExtensions.Shake(GetComponent<RectTransform>(), combatManager.shakeIntensity + 20f, combatManager.shakeDuration * 3);
        }
        else if (isCrit)
        {
            UIExtensions.Shake(combatManager.entitySprite.rectTransform, combatManager.shakeIntensity + 7f, combatManager.shakeDuration * 2);
            UIExtensions.Shake(GetComponent<RectTransform>(), combatManager.shakeIntensity + 7f, combatManager.shakeDuration * 2);
        }
        else
        {
            UIExtensions.Shake(combatManager.entitySprite.rectTransform, combatManager.shakeIntensity, combatManager.shakeDuration);
            UIExtensions.Shake(GetComponent<RectTransform>(), combatManager.shakeIntensity + 7f, combatManager.shakeDuration * 2);
        }
        combatManager.UpdateHealthUI(false);

        string critText = isCrit ? "!" : "";
        combatManager.damageText.text = damage.ToString() + critText;

        UIExtensions.Flash(combatManager.entitySprite);
        UIExtensions.GenerateDamageText(combatManager.damageText, isCrit, damage);
    }
    public void OnGuardChanged()
    {
        float mShield = combatManager.maxShield > 0 ? combatManager.maxShield : 1f;
        float calculatedValue = Mathf.Clamp((float)combatManager.guardAmount / mShield, 0f, 1f);

        UIExtensions.GhostBarFill(guardBar, guardBarGhost, calculatedValue);
        if (rsm != null) rsm.quickStatsPanel.PlayerQuickStats(context);
    }

    public void OnFocusChanged()
    {
        float fCap = combatManager.focusCap > 0 ? combatManager.focusCap : 1f;
        float calculatedValue = Mathf.Clamp((float)combatManager.focusAmount / fCap, 0f, 1f);
        Debug.Log($"Focus Amount: {combatManager.focusAmount}, Focus Cap: {combatManager.focusCap}, Calculated Value: {calculatedValue}");
        UIExtensions.GhostBarFill(focusBar, focusBarGhost, calculatedValue);
        if (rsm != null) rsm.quickStatsPanel.PlayerQuickStats(context);
    }
    #endregion
}
