using Assets._Project.Scripts.UI;
using TextBasedRPG.Events;
using TextBasedRPG.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [Header("Class Avatar & Title")]
    [SerializeField] public Image playerAvatar;
    [SerializeField] private TMP_Text playerClass;

    [Header("HP Bar")]
    [SerializeField] private TMP_Text playerHPValue;
    [SerializeField] private Image playerGhostHPFill;
    [SerializeField] private Image playerHPFill;
    
    [Header("EXP Bar")]
    [SerializeField] private TMP_Text playerEXPValue;
    [SerializeField] private Image playerGhostEXPFill;
    [SerializeField] private Image playerEXPFill;

    [Header("Misc Info")]
    [SerializeField] private TMP_Text playerLevel;
    [SerializeField] private TMP_Text playerGold;
    [SerializeField] private TMP_Text playerLocation;

    [Header("Damage Text")]
    [SerializeField] public TMP_Text damageText;

    #region Enable & Disable
    private void OnEnable()
    {
        EventManager.HeroEvents.OnGoldChanged += UpdateGoldUI;
        EventManager.HeroEvents.OnExpChanged += UpdatExpUI;
        EventManager.HeroEvents.OnHPValueChanged += UpdateHPUI;
    }

    private void OnDisable()
    {
        EventManager.HeroEvents.OnGoldChanged -= UpdateGoldUI;
        EventManager.HeroEvents.OnExpChanged -= UpdatExpUI;
        EventManager.HeroEvents.OnHPValueChanged -= UpdateHPUI;
    }
    #endregion

    #region Event Handlers
    public void UpdateGoldUI(GameContext context)
    {
        playerGold.text = $"{context.Player.Gold}";

        LeanTween.scale(playerGold.gameObject, Vector3.one * 1.2f, 0.1f).setLoopPingPong(1);
    }
    public void UpdatExpUI(GameContext context)
    {
        float targetFill = (float)context.Player.CurExp / context.Player.ReqExp;
        playerLevel.text = $"{context.Player.Level}";
        playerEXPValue.text = $"{context.Player.CurExp}/{context.Player.ReqExp}";

        if (targetFill < playerEXPFill.fillAmount)
        {
            playerEXPFill.fillAmount = 0f;
            playerGhostEXPFill.fillAmount = 0f;
        }

        UIExtensions.GhostBarFill(playerEXPFill, playerGhostEXPFill, targetFill);

        LeanTween.scale(playerEXPValue.gameObject, Vector3.one * 1.03f, 0.1f).setLoopPingPong(1);
        LeanTween.scale(playerEXPFill.transform.parent.transform.parent.gameObject, Vector3.one * 1.03f, 0.1f).setLoopPingPong(1);
    }
    public void UpdateHPUI(GameContext context)
    {
        if (context.Player.CurHP >= context.Player.TotalHP)
            context.Player.CurHP = context.Player.TotalHP;
        
        if (context.Player.CurHP <= context.Player.TotalHP * 0.3f)
        {
            //vignette here later
            EventManager.CombatEvents.TriggerOnPlayerLowHP();
        }
        float targetFill = (float)context.Player.CurHP / context.Player.TotalHP;
        playerHPValue.text = $"{context.Player.CurHP}/{context.Player.TotalHP}";

        UIExtensions.GhostBarFill(playerHPFill, playerGhostHPFill, targetFill);

        LeanTween.scale(playerHPValue.gameObject, Vector3.one * 1.03f, 0.1f).setLoopPingPong(1);
        LeanTween.scale(playerHPFill.transform.parent.transform.parent.gameObject, Vector3.one * 1.03f, 0.1f).setLoopPingPong(1);
    }
    #endregion

    public void PlayerCard(GameContext context)
    {
        var heroDb = Resources.Load<HeroDatabase>("HeroDatabase");

        playerAvatar.sprite = heroDb.GetHeroByName(context.Player.ClassName).classIcon;
        playerClass.text = context.Player.ClassName;

        playerHPValue.text = $"{context.Player.CurHP}/{context.Player.TotalHP}";
        playerHPFill.fillAmount = (float)context.Player.CurHP / context.Player.TotalHP;
        playerGhostHPFill.fillAmount = (float)context.Player.CurHP / context.Player.TotalHP;

        playerEXPValue.text = $"{context.Player.CurExp}/{context.Player.ReqExp}";
        playerEXPFill.fillAmount = (float)context.Player.CurExp / context.Player.ReqExp;
        playerGhostEXPFill.fillAmount = (float)context.Player.CurExp / context.Player.ReqExp;

        playerLevel.text = $"{context.Player.Level}";
        UpdateGoldUI(context);

        if (context.Player.ActiveLocation != null)
            playerLocation.text = LocationManager.locations[context.Player.ActiveLocation];
        else
            playerLocation.text = "Unknown";
    }
}
