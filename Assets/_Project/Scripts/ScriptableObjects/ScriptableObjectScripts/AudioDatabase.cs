using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDatabase", menuName = "RPG/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    [Header("Music Clips")]
    public List<AudioClip> regions;

    [Header("SFX Clips")]
    [Header("Player Clips")]
    public List<AudioClip> gettingHit;

    [Header("Monster Clips")]
    public List<AudioClip> slimeHit;
    public List<AudioClip> wolfHit;
    public List<AudioClip> skeletonHit;
}