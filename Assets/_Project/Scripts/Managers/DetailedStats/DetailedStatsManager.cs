using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Managers;
using TMPro;
using UnityEngine;

public class DetailedStatsManager : MonoBehaviour
{
    #region Variables
    [Header("Main Stats")]
    [SerializeField] private TMP_Text atk;
    [SerializeField] private TMP_Text def;
    [SerializeField] private TMP_Text spd;
    [SerializeField] private TMP_Text critRate;
    [SerializeField] private TMP_Text critDamage;
    [SerializeField] private TMP_Text totalExp;
    [SerializeField] private TMP_Text latestRegion;
    [Header("Training Stats")]
    [SerializeField] private TMP_Text str;
    [SerializeField] private TMP_Text dex;
    [SerializeField] private TMP_Text vit;
    [SerializeField] private TMP_Text agi;
    [Header("Combat Stats")]
    [SerializeField] private TMP_Text deaths;
    [SerializeField] private TMP_Text entitiesSlayed;
    [SerializeField] private TMP_Text heaviestDamage;
    #endregion

    #region OnEnable & OnDisable
    private void OnEnable()
    {
        GameContext context = GameManager.Instance.Context;
        Hero p = context.Player;
        string locationName = LocationManager.GetLocationName(context.Player.ActiveLocation);

        atk.text = p.TotalATK.ToString();
        def.text = p.TotalDEF.ToString();
        spd.text = p.TotalSPD.ToString();
        critRate.text = p.CritRate.ToString();
        critDamage.text = p.CritDamage.ToString();
        totalExp.text = p.TotalExp.ToString();
        latestRegion.text = $"{locationName} ({p.UnlockedUntill})";

        str.text = p.InvestedSTRPoints.ToString();
        dex.text = p.InvestedDEXPoints.ToString();
        agi.text = p.InvestedAGIPoints.ToString();
        vit.text = p.InvestedVITPoints.ToString();

    }
    private void OnDisable()
    {

    }
    #endregion
}
