using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TextBasedRPG.Events;
using Assets._Project.Scripts.UI;

/// <summary>
/// Görev ekranındaki her bir kartı temsil eder.
/// Inspector'dan questId atanır; QuestManager'dan ilgili görevi çekerek kendini doldurur.
/// QuestManager veya başka sistemlerle doğrudan bağımlılık yoktur: sadece event dinler.
/// </summary>
public class QuestCardUI : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────
    [Header("Görev Kimliği")]
    [Tooltip("Bu kartın gösterdiği görevin MilestoneQuest.Id değeri. Örn: Q_MONSTER")]
    [SerializeField] private string questId;

    [Header("UI Referansları")]
    [SerializeField] private TMP_Text  titleText;
    [SerializeField] private TMP_Text  descriptionText;
    [SerializeField] private TMP_Text  progressText;   // "15 / 100"
    [SerializeField] private TMP_Text  rewardText;     // "5K"
    [SerializeField] private Image     progressBar;  // Image Type: Filled, Fill Method: Horizontal
    [SerializeField] private Image     ghostBar;  // Image Type: Filled, Fill Method: Horizontal
    [SerializeField] private Button    claimButton;

    // ── Cache ─────────────────────────────────────────────────────
    private MilestoneQuest _quest;

    // ── Unity Lifecycle ───────────────────────────────────────────
    private void OnEnable()
    {
        EventManager.QuestEvents.OnQuestUIRefresh += RefreshUI;

        // Sahneye açıldığında/etkinleştiğinde hemen çiz
        RefreshUI();
    }

    private void OnDisable()
    {
        EventManager.QuestEvents.OnQuestUIRefresh -= RefreshUI;
    }

    // ── Refresh ───────────────────────────────────────────────────

    /// <summary>
    /// Kartı QuestManager'daki güncel verilerle yeniden çizer.
    /// EventManager.QuestEvents.OnQuestUIRefresh event'i tetiklendiğinde otomatik çağrılır.
    /// </summary>
    private void RefreshUI()
    {
        // İlk çağrıda veya manager henüz hazır değilse çekmeye çalış
        if (_quest == null && QuestManager.Instance != null)
            _quest = QuestManager.Instance.GetQuestById(questId);

        if (_quest == null)
        {
            Debug.LogWarning($"[QuestCardUI] '{questId}' ID'li görev bulunamadı. Inspector'u kontrol et.");
            return;
        }

        // ── Başlık & Açıklama ─────────────────────────────────────
        if (titleText       != null) titleText.text       = _quest.Title;
        if (descriptionText != null) descriptionText.text = _quest.Description;

        // ── İlerleme Hesabı ───────────────────────────────────────
        // Görev maxlanmışsa son hedefi referans al; aksi hâlde mevcut tier hedefini kullan.
        int displayTarget = _quest.IsMaxed
            ? _quest.Targets[_quest.Targets.Length - 1]
            : _quest.CurrentTarget;

        int displayProgress = Mathf.Min(_quest.Progress, displayTarget);
        float fillRatio     = Mathf.Clamp01((float)_quest.Progress / displayTarget);

        // ── Progress Bar ──────────────────────────────────────────
        //if (progressBar != null)
            //progressBar.fillAmount = fillRatio;

        if (progressBar != null)
            UIExtensions.GhostBarFill(progressBar,ghostBar,fillRatio);

        // ── Progress Metni ────────────────────────────────────────
            if (progressText != null)
        {
            if (_quest.IsMaxed)
                progressText.text = $"{displayTarget} / {displayTarget}";
            else
                progressText.text = $"{displayProgress} / {displayTarget}";
        }

        // ── Ödül Metni ────────────────────────────────────────────
        if (rewardText != null)
            rewardText.text = _quest.IsMaxed
                ? "MAX"
                : ToAbbreviated(_quest.CalculatedReward);

        // ── Claim Butonu ──────────────────────────────────────────
        if (claimButton != null)
            claimButton.interactable = _quest.IsClaimable;
    }

    // ── Buton Callback ────────────────────────────────────────────

    /// <summary>
    /// Inspector'da Claim butonunun OnClick eventine bu metodu bağla.
    /// Sequential Claim: Bir sonraki tier de hazırsa buton aktif kalmaya devam eder.
    /// </summary>
    public void OnClaimButtonClicked()
    {
        if (QuestManager.Instance == null) return;

        QuestManager.Instance.ClaimReward(questId);
        // RefreshUI, QuestManager içinden TriggerQuestUIRefresh() ile tetiklenecek.
        // Manuel çağrıya gerek yok; ancak güvenlik için burada da tetikleyebilirsin:
        // RefreshUI();
    }

    // ── Yardımcı ─────────────────────────────────────────────────

    /// <summary>Büyük sayıları kısaltır. Örn: 5000 → "5K", 1500000 → "1.5M"</summary>
    private static string ToAbbreviated(int value)
    {
        if (value >= 1_000_000) return $"{value / 1_000_000f:0.#}M";
        if (value >= 1_000)     return $"{value / 1_000f:0.#}K";
        return value.ToString();
    }
}
