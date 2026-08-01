using TMPro;
using UnityEngine;

public class QuickStatsPanel : MonoBehaviour
{
    [Header("Quick Stats")]
    [SerializeField] public TMP_Text attackVal;
    [SerializeField] public TMP_Text defenseVal;
    [SerializeField] public TMP_Text speedVal;
    [SerializeField] public TMP_Text critRateVal;
    [SerializeField] public TMP_Text critDmgVal;
    public void PlayerQuickStats(GameContext context)
    {
        attackVal.text = $"{context.Player.BonuslessATK} + <color=#8FCE00>{context.Player.BonusATK}</color>";
        defenseVal.text = $"{context.Player.BonuslessDEF} + <color=#8FCE00>{context.Player.BonusDEF}</color>";
        speedVal.text = $"{context.Player.BonuslessSPD} + <color=#8FCE00>{context.Player.BonusSPD}</color>";
        critRateVal.text = $"{context.Player.BonuslessCritRate} + <color=#8FCE00>{context.Player.BonusCritRate}</color>%";
        critDmgVal.text = $"{context.Player.BonuslessCritDamage} + <color=#8FCE00>{context.Player.BonusCritDMG}</color>%";
    }
}
