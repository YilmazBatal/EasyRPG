using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EnemyAudioEntry
{
    public string EntityTypeID; // "Slime", "Wolf"
    public List<AudioClip> hitSounds;
}

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "RPG/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    [Header("Region Clips | Music")]
    public List<AudioClip> regions;

    [Header("Player Clips | SFX")]
    public List<AudioClip> gettingHit;

    [Header("Enemy Clips | SFX")]
    public List<EnemyAudioEntry> enemyHitSounds;

    [Header("UI Clips | UI")]
    public AudioClip typewriter;

    private Dictionary<string, List<AudioClip>> _hitSoundCache;

    public void Initialize()
    {
        _hitSoundCache = new Dictionary<string, List<AudioClip>>();

        foreach (var entry in enemyHitSounds)
        {
            if (!string.IsNullOrEmpty(entry.EntityTypeID) && !_hitSoundCache.ContainsKey(entry.EntityTypeID))
            {
                _hitSoundCache.Add(entry.EntityTypeID, entry.hitSounds);
            }
        }
    }

    public AudioClip GetHitSound(string typeID)
    {
        if (string.IsNullOrEmpty(typeID)) return null;

        if (_hitSoundCache == null) Initialize();

        if (_hitSoundCache.TryGetValue(typeID, out List<AudioClip> clips))
            return clips[Random.Range(0, clips.Count)];

        Debug.LogWarning($"[AudioDB] {typeID} not in the dictionary");
        return null;
    }
}