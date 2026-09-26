using System.Collections.Generic;

// ─────────────────────────────────────────────
// Görev Türleri
// ─────────────────────────────────────────────
public enum QuestType
{
    MonsterKill,
    EliteKill,
    ItemUpgrade,
    Adventure,
    Trade,
    DamageRecord
}

// ─────────────────────────────────────────────
// Save dosyasına yazılacak minimal DTO.
// DataManager.MapContextToData() içinde Data nesnesine eklenir,
// DynamicData.LoadPlayerData() içinde geri yüklenir.
// ─────────────────────────────────────────────
[System.Serializable]
public class QuestSaveData
{
    public string Id          { get; set; }
    public int    Progress    { get; set; }
    public int    CurrentTier { get; set; }
}

// ─────────────────────────────────────────────
// Çalışma zamanı görev nesnesi.
// Inspector'dan veya ScriptableObject üzerinden doldurulur.
// ─────────────────────────────────────────────
[System.Serializable]
public class MilestoneQuest
{
    // ── Statik Tanım (Inspector'dan doldurulur) ──────────────────
    /// <summary>Görevin benzersiz kimliği. Örn: "Q_MONSTER"</summary>
    public string    Id;

    /// <summary>UI'da gösterilecek başlık. Örn: "Monster Sweep"</summary>
    public string    Title;

    /// <summary>UI'da gösterilecek açıklama.</summary>
    public string    Description;

    public QuestType Type;

    /// <summary>
    /// Kümülatif hedefler dizisi. Örn: { 10, 50, 100, 250, 500 }
    /// Her eleman, o aşamayı tamamlamak için gereken TOPLAM ilerleme miktarını gösterir.
    /// </summary>
    public int[]     Targets;

    /// <summary>1. aşama için taban ödül. Her tier'da (tier+1) ile çarpılır.</summary>
    public int       BaseRewardGold;

    // ── Dinamik Durum (Save/Load tarafından doldurulur) ──────────
    /// <summary>
    /// ASLA sıfırlanmaz. Oyuncunun bu görev türünde gerçekleştirdiği toplam eylem sayısı.
    /// DamageRecord için bu, o ana kadar kaydedilen en yüksek hasar değeridir.
    /// </summary>
    public int Progress    { get; set; }

    /// <summary>Oyuncunun ödülünü aldığı en son tier indeksi + 1. 0 = henüz hiç alınmadı.</summary>
    public int CurrentTier { get; set; }

    // ── Hesaplanan Özellikler ─────────────────────────────────────
    /// <summary>Tüm tier'lar bitti mi?</summary>
    public bool IsMaxed => CurrentTier >= Targets.Length;

    /// <summary>
    /// Sıradaki talep edilmemiş tier'ın hedef değeri.
    /// Görev maxlanmışsa son hedefi döndürür (güvenlik için).
    /// </summary>
    public int CurrentTarget =>
        CurrentTier < Targets.Length ? Targets[CurrentTier] : Targets[Targets.Length - 1];

    /// <summary>
    /// Oyuncu şu an ödül talep edebilir mi?
    /// Maxed değilse ve progress mevcut tier hedefine ulaştıysa true.
    /// Sequential Claim: birden fazla tier birden hazırsa her Claim çağrısında bir tier geçer.
    /// </summary>
    public bool IsClaimable => !IsMaxed && Progress >= CurrentTarget;

    /// <summary>Mevcut tier için hesaplanmış ödül miktarı.</summary>
    public int CalculatedReward => BaseRewardGold * (CurrentTier + 1);

    // ── Save / Load yardımcıları ──────────────────────────────────
    public QuestSaveData ToSaveData() => new QuestSaveData
    {
        Id          = Id,
        Progress    = Progress,
        CurrentTier = CurrentTier,
    };

    public void ApplySaveData(QuestSaveData data)
    {
        Progress    = data.Progress;
        CurrentTier = data.CurrentTier;
    }
}
