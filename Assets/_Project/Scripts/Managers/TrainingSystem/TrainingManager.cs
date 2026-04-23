using TextBasedRPG.Core.Heroes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingManager : MonoBehaviour
{
    #region Variables
    [SerializeField] TMP_Text totalAllocated;
    [SerializeField] TMP_Text unusedPoints;

    [Header("Allocation Buttons & Values & Input Fields")]
    [SerializeField] Button STR;
    [SerializeField] Button DEX;
    [SerializeField] Button VIT;
    [SerializeField] Button AGI;

    [SerializeField] TMP_Text STRValue;
    [SerializeField] TMP_Text DEXValue;
    [SerializeField] TMP_Text VITValue;
    [SerializeField] TMP_Text AGIValue;

    [SerializeField] TMP_InputField STRInput;
    [SerializeField] TMP_InputField DEXInput;
    [SerializeField] TMP_InputField VITInput;
    [SerializeField] TMP_InputField AGIInput;
    
    Hero p;
    #endregion

    private void OnEnable()
    {
        AddListeners(); 

        p = GameManager.Instance.Context.Player;

        UpdateUI();
    }

    private void OnDisable()
    {
        RemoveListeners();
    }
    private void AllocatePoints(StatType stat, TMP_InputField points)
    {
        if (string.IsNullOrEmpty(points.text) || !int.TryParse(points.text, out int pointsToInvest))
        {
            Toaster.Instance.ShowToast("Put an actual number please.", null);
            return;
        }

        if (pointsToInvest > 0 && p.UnusedStatPoints >= pointsToInvest)
        {
            switch (stat)
            {
                case StatType.STR: p.InvestedSTRPoints += pointsToInvest; break;
                case StatType.DEX: p.InvestedDEXPoints += pointsToInvest; break;
                case StatType.VIT: p.InvestedVITPoints += pointsToInvest; break;
                case StatType.AGI: p.InvestedAGIPoints += pointsToInvest; break;
            }
            p.UnusedStatPoints -= pointsToInvest;
            UpdateUI();
            Toaster.Instance.ShowToast($"{pointsToInvest} points invested in {stat}. You can feel that you are getting stronger.", null);
            GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);
        }
        
    }
    void UpdateUI()
    {
        unusedPoints.text = p.UnusedStatPoints.ToString();
        totalAllocated.text = (p.InvestedSTRPoints + p.InvestedDEXPoints + p.InvestedVITPoints + p.InvestedAGIPoints).ToString();

        STRValue.text = $"TOTAL : {p.InvestedSTRPoints}";
        DEXValue.text = $"TOTAL : {p.InvestedDEXPoints}";
        VITValue.text = $"TOTAL : {p.InvestedVITPoints}";
        AGIValue.text = $"TOTAL : {p.InvestedAGIPoints}";
    }

    private void AddListeners()
    {
        STR.onClick.AddListener(() => AllocatePoints(StatType.STR, STRInput));
        DEX.onClick.AddListener(() => AllocatePoints(StatType.DEX, DEXInput));
        VIT.onClick.AddListener(() => AllocatePoints(StatType.VIT, VITInput));
        AGI.onClick.AddListener(() => AllocatePoints(StatType.AGI, AGIInput));
    }
    private void RemoveListeners()
    {
        STR.onClick.RemoveAllListeners();
        DEX.onClick.RemoveAllListeners();
        VIT.onClick.RemoveAllListeners();
        AGI.onClick.RemoveAllListeners();
    }

}
