using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuickStatsPanel : MonoBehaviour
{
    [Header("Quick Stats")]
    [SerializeField] private TMP_Text attackVal;
    [SerializeField] private TMP_Text defenseVal;
    [SerializeField] private TMP_Text speedVal;
    [SerializeField] private TMP_Text critRateVal;
    [SerializeField] private TMP_Text critDmgVal;

    [SerializeField] private Color bonusColor = new Color(0.5607843f, 0.8078431f, 0f);
    private string PrintBonusStat(float bonus)
    {
        return $"<color=#{bonusColor.ToHexString()}>{bonus}</color>";
    }
    public void PlayerQuickStats(GameContext context)
    {
        attackVal.text = $"{context.Player.BonuslessATK}{(context.Player.BonusATK != 0 ? $" + {PrintBonusStat(context.Player.BonusATK)}" : "")}";
        defenseVal.text = $"{context.Player.BonuslessDEF}{(context.Player.BonusDEF != 0 ? $" + {PrintBonusStat(context.Player.BonusDEF)}" : "")}";
        speedVal.text = $"{context.Player.BonuslessSPD}{(context.Player.BonusSPD != 0 ? $" + {PrintBonusStat(context.Player.BonusSPD)}" : "")}";
        critRateVal.text = $"{context.Player.BonuslessCritRate}{(context.Player.BonusCritRate != 0 ? $" + {PrintBonusStat(context.Player.BonusCritRate)}" : "")}%";
        critDmgVal.text = $"{context.Player.BonuslessCritDamage}{(context.Player.BonusCritDMG != 0 ? $" + {PrintBonusStat(context.Player.BonusCritDMG)}" : "")}%";
    }
}
