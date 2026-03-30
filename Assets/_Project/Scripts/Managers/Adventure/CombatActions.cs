using System.Collections;
using TextBasedRPG.Managers;
using TextBasedRPG.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatActions : MonoBehaviour
{
    [Header("CM & UI References")]
    [SerializeField] CombatManager combatManager;
    [SerializeField] TMP_Text contentText;
    [SerializeField] public Button attackBtn;
    [SerializeField] public Button focusBtn;
    [SerializeField] public Button guardBtn;
    [SerializeField] public Button backpackBtn;
    [SerializeField] public Button fleeBtn;

    GameContext context;
    private void Start() => context = GameManager.Instance.Context;

    #region Attack
    public void OnAttackClicked()
    {
        if (!combatManager.isPlayerTurn) return;
        StartCoroutine(PlayerAttackRoutine());
    }

    private IEnumerator PlayerAttackRoutine()
    {
        combatManager.SetButtonsInteractable(false);

        IDamageCalculator damage = new DamageCalculator();
        int calculatedDamage = damage.CalculateDMG(
            context.Player.TotalATK,
            combatManager.generatedEnemy.TotalDEF,
            context.Player.CritRate,
            context.Player.CritDamage,
            out bool isCrit);

        string critText = isCrit ? "<color=#0F172A>Critical hit!</color>" : "";

        combatManager.generatedEnemy.CurHP -= calculatedDamage;

        contentText.text = string.Empty;

        StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText,combatManager.GetEnemyStatus(combatManager.generatedEnemy)));

        StartCoroutine(UIManager.BruteForceTypeWriterRoutine(
            contentText,
            $"You dealt <color=#0F172A>{calculatedDamage}</color> to {combatManager.generatedEnemy.Name}! {critText}"));

        combatManager.UpdateHealthUI(false);

        yield return new WaitForSeconds(2f);

        if (combatManager.generatedEnemy.CurHP <= 0)
        {
            combatManager.EndCombat(true);
        }
        else
        {
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
        combatManager.SetButtonsInteractable(false);

        int damage = Mathf.Max(1, GameManager.Instance.Context.Player.TotalATK - combatManager.generatedEnemy.TotalDEF);
        combatManager.generatedEnemy.CurHP -= damage;
        contentText.text = string.Empty;
        StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"<color=#0F172A>{combatManager.generatedEnemy.Name}</color> dealt {damage} damage to you!"));

        combatManager.UpdateHealthUI(false);

        yield return new WaitForSeconds(1.2f);

        if (combatManager.generatedEnemy.CurHP <= 0)
        {
            combatManager.EndCombat(true);
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
            combatManager.EndCombat(true);
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
            combatManager.EndCombat(true);
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

        int damage = Mathf.Max(1, GameManager.Instance.Context.Player.TotalATK - combatManager.generatedEnemy.TotalDEF);
        combatManager.generatedEnemy.CurHP -= damage;
        contentText.text = string.Empty;
        StartCoroutine(UIManager.BruteForceTypeWriterRoutine(contentText, $"<color=#0F172A>{combatManager.generatedEnemy.Name}</color> dealt {damage} damage to you!"));

        combatManager.UpdateHealthUI(false);

        yield return new WaitForSeconds(1.2f);

        if (combatManager.generatedEnemy.CurHP <= 0)
        {
            combatManager.EndCombat(true);
        }
        else
        {
            combatManager.isPlayerTurn = false;
            StartCoroutine(combatManager.EnemyTurnRoutine());
        }
    }
    #endregion
}
