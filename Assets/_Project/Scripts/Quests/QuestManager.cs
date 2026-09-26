using System.Collections.Generic;
using System.Linq;
using TextBasedRPG.Events;
using UnityEngine;

/// <summary>
/// Görev sisteminin beyni. Sahnede tek bir instance olmalıdır (Singleton).
///
/// Bağımlılık yok: Savaş/Blacksmith/Ticaret sistemleri bu sınıfı doğrudan import etmez;
/// sadece EventManager.QuestEvents üzerinden event tetikler.
/// </summary>
public class QuestManager : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────
    public static QuestManager Instance { get; private set; }

    // ── Inspector ─────────────────────────────────────────────────
    [Header("Quest Definitions (Inspector'dan doldur)")]
    [SerializeField] private List<MilestoneQuest> _questDefinitions = new();

    // ── Runtime ───────────────────────────────────────────────────
    /// <summary>Dışarıdan (UI kartları) okuma için salt okunur erişim.</summary>
    public IReadOnlyList<MilestoneQuest> Quests => _questDefinitions;

    // ── Unity Lifecycle ───────────────────────────────────────────
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void OnEnable()
    {
        EventManager.QuestEvents.OnQuestProgressed += HandleQuestProgress;
    }

    private void OnDisable()
    {
        EventManager.QuestEvents.OnQuestProgressed -= HandleQuestProgress;
    }

    private void Start()
    {
        // GameManager tamamen yüklendikten sonra kayıtlı progress değerlerini uygula.
        LoadQuestProgress();
    }

    // ── Save / Load ───────────────────────────────────────────────

    /// <summary>
    /// GameContext üzerindeki QuestSaveData listesini görev tanımlarına uygular.
    /// GameManager.Start() çağrıldıktan sonra (yani Context yüklendikten sonra) çalışır.
    /// </summary>
    public void LoadQuestProgress()
    {
        var context = GameManager.Instance?.Context;
        if (context == null) return;

        List<QuestSaveData> savedQuests = context.QuestSaves;
        if (savedQuests == null || savedQuests.Count == 0) return;

        foreach (var quest in _questDefinitions)
        {
            var saved = savedQuests.FirstOrDefault(s => s.Id == quest.Id);
            if (saved != null)
                quest.ApplySaveData(saved);
        }
    }

    /// <summary>
    /// Mevcut görev ilerlemelerini Context'e yazar ve SaveService ile diske kaydeder.
    /// </summary>
    public void SaveQuestProgress()
    {
        var context = GameManager.Instance?.Context;
        if (context == null) return;

        context.QuestSaves = _questDefinitions
            .Select(q => q.ToSaveData())
            .ToList();

        GameManager.Instance.SaveService.SaveGame(context);
    }

    // ── Event Handler ─────────────────────────────────────────────

    /// <summary>
    /// Herhangi bir oyun sistemi <c>EventManager.QuestEvents.TriggerQuestProgressed</c>
    /// çağırdığında bu metot tetiklenir. QuestManager dışında hiçbir şey bilmek zorunda değil.
    /// </summary>
    private void HandleQuestProgress(QuestType type, int amount)
    {
        bool anyChanged = false;

        foreach (var quest in _questDefinitions.Where(q => q.Type == type))
        {
            if (quest.IsMaxed) continue;

            if (type == QuestType.DamageRecord)
            {
                // Rekor tipi: sadece yeni değer eskisinden büyükse güncelle
                if (amount > quest.Progress)
                {
                    quest.Progress = amount;
                    anyChanged = true;
                }
            }
            else
            {
                // Kümülatif tip: her zaman üstüne ekle
                quest.Progress += amount;
                anyChanged = true;
            }
        }

        // Herhangi bir değişiklik olduysa aktif UI kartlarını bilgilendir
        if (anyChanged)
            EventManager.QuestEvents.TriggerQuestUIRefresh();
    }

    // ── Claim (Ödül Alma) ─────────────────────────────────────────

    /// <summary>
    /// UI'daki "Claim" butonuna basıldığında çağrılır.
    /// Sequential Claim: Birden fazla tier hazırsa, her çağrıda yalnızca bir tier ilerler.
    /// Buton aktif kalırsa tekrar basılabilir.
    /// </summary>
    /// <param name="questId">MilestoneQuest.Id değeri. Örn: "Q_MONSTER"</param>
    public void ClaimReward(string questId)
    {
        var quest = _questDefinitions.Find(q => q.Id == questId);

        if (quest == null)
        {
            Debug.LogWarning($"[QuestManager] ClaimReward: '{questId}' ID'li görev bulunamadı.");
            return;
        }

        if (!quest.IsClaimable)
        {
            Debug.LogWarning($"[QuestManager] ClaimReward: '{questId}' henüz talep edilebilir değil.");
            return;
        }

        var context = GameManager.Instance?.Context;
        if (context?.Player == null)
        {
            Debug.LogError("[QuestManager] ClaimReward: GameContext veya Player null!");
            return;
        }

        // 1. Ödülü ver
        int reward = quest.CalculatedReward;
        context.Player.Gold += reward;
        EventManager.HeroEvents.TriggerGoldChanged(context);

        // 2. Tier'ı ilerlet
        quest.CurrentTier++;

        Debug.Log($"[QuestManager] '{quest.Title}' | Tier {quest.CurrentTier} tamamlandı. +{reward} Altın.");

        // 3. Kaydet (hem quest verisi hem altın birlikte)
        SaveQuestProgress();

        // 4. UI'ı yenile
        EventManager.QuestEvents.TriggerQuestUIRefresh();
    }

    // ── Yardımcı ─────────────────────────────────────────────────

    /// <summary>ID ile görev arar. UI kartları için kullanışlı.</summary>
    public MilestoneQuest GetQuestById(string questId) =>
        _questDefinitions.Find(q => q.Id == questId);
}
