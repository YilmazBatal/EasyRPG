using Assets._Project.Scripts.UI;
using System;
using System.Collections;
using TextBasedRPG.Core.Entities;
using TextBasedRPG.Events;
using TextBasedRPG.Managers;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CombatActions : MonoBehaviour
{
    #region References
    [Header("CM & UI References")]
    [SerializeField] CombatManager combatManager;
    [SerializeField] TMP_Text contentText;
    [SerializeField] public Button attackBtn;
    [SerializeField] public Button focusBtn;
    [SerializeField] public Button guardBtn;
    [SerializeField] public Button backpackBtn;
    [SerializeField] public Button fleeBtn;
    GameContext context;
    #endregion

    #region Enable & Disable
    private void Start() => context = GameManager.Instance.Context;
    private void OnEnable()
    {
        EventManager.CombatEvents.OnEntityGotHit += OnEntityGotHit;
    }
    private void OnDisable()
    {
        EventManager.CombatEvents.OnEntityGotHit -= OnEntityGotHit;
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
        combatManager.SetButtonsInteractable(false);

        float hitAccuracy = Random.value;
        if (hitAccuracy >= 0.95f)
        {
            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"{combatManager.generatedEnemy.Name} somehow dodged this."));
            yield return new WaitForSeconds(2f);
            combatManager.isPlayerTurn = false;
            StartCoroutine(combatManager.EnemyTurnRoutine());
        }
        else
        {
            IDamageCalculator damage = new DamageCalculator();
            int calculatedDamage = damage.CalculateDMG(
                context.Player.TotalATK,
                combatManager.generatedEnemy.TotalDEF,
                context.Player.CritRate,
                context.Player.CritDamage,
                out bool isCrit);

            if (calculatedDamage >= combatManager.generatedEnemy.CurHP)
            {
                calculatedDamage = combatManager.generatedEnemy.CurHP;
                combatManager.generatedEnemy.CurHP = calculatedDamage;
            }
            else
                combatManager.generatedEnemy.CurHP -= calculatedDamage;

            string critText = isCrit ? "<color=#0F172A>Critical hit!</color>" : "";

            EventManager.CombatEvents.TriggerOnEntityGotHit(isCrit, calculatedDamage);

            contentText.text = string.Empty;

            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText,combatManager.GetEnemyStatus(combatManager.generatedEnemy)));

            yield return new WaitForSeconds(2f);

            StartCoroutine(UIManager.BruteForceTypeWriterRoutine(
                contentText,
                $"You dealt <color=#0F172A>{calculatedDamage}</color> to {combatManager.generatedEnemy.Name}! {critText}"));

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
        combatManager.SetButtonsInteractable(false);

        int damage = Mathf.Max(1, GameManager.Instance.Context.Player.TotalATK - combatManager.generatedEnemy.TotalDEF);
        combatManager.generatedEnemy.CurHP -= damage;
        contentText.text = string.Empty;
        StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"<color=#0F172A>{combatManager.generatedEnemy.Name}</color> dealt {damage} damage to you!"));

        combatManager.UpdateHealthUI(false);

        yield return new WaitForSeconds(1.2f);

        if (combatManager.generatedEnemy.CurHP <= 0)
        {
            combatManager.EndCombat(CombatResult.Victory);
        }
        else
        {
            combatManager.isPlayerTurn = false;
            StartCoroutine(combatManager.EnemyTurnRoutine());
        }
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
        combatManager.SetButtonsInteractable(false);

        int damage = Mathf.Max(1, GameManager.Instance.Context.Player.TotalATK - combatManager.generatedEnemy.TotalDEF);
        combatManager.generatedEnemy.CurHP -= damage;
        contentText.text = string.Empty;
        StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"<color=#0F172A>{combatManager.generatedEnemy.Name}</color> dealt {damage} damage to you!"));

        combatManager.UpdateHealthUI(false);

        yield return new WaitForSeconds(1.2f);

        if (combatManager.generatedEnemy.CurHP <= 0)
        {
            combatManager.EndCombat(CombatResult.Victory);
        }
        else
        {
            combatManager.isPlayerTurn = false;
            StartCoroutine(combatManager.EnemyTurnRoutine());
        }
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
        combatManager.SetButtonsInteractable(false);

        int damage = Mathf.Max(1, GameManager.Instance.Context.Player.TotalATK - combatManager.generatedEnemy.TotalDEF);
        combatManager.generatedEnemy.CurHP -= damage;
        contentText.text = string.Empty;
        StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"<color=#0F172A>{combatManager.generatedEnemy.Name}</color> dealt {damage} damage to you!"));

        combatManager.UpdateHealthUI(false);

        yield return new WaitForSeconds(1.2f);

        if (combatManager.generatedEnemy.CurHP <= 0)
        {
            combatManager.EndCombat(CombatResult.Victory);
        }
        else
        {
            combatManager.isPlayerTurn = false;
            StartCoroutine(combatManager.EnemyTurnRoutine());
        }
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
            yield return new WaitForSeconds(2f);
            
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
    private void OnEntityGotHit(bool isCrit, int damage)
    {
        if (isCrit)
        {
            UIExtensions.Shake(combatManager.entitySprite.rectTransform,combatManager.shakeIntensity + 7f, combatManager.shakeDuration * 2);
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

    #endregion
}
