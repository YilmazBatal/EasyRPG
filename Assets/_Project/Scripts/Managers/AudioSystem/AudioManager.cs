using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] public AudioDatabase audioDB;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateMixerVolume(string parameterName, float sliderValue)
    {
        // To avoid log(0) which is undefined, we clamp the slider value to a minimum of 0.0001.
        float dB = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        mainMixer.SetFloat(parameterName, dB);
    }
    public void PlaySFX(AudioClip clip, float pitch = 1.0f)
    {
        if (clip == null) return;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayUI(AudioClip clip, float pitch = 1.0f)
    {
        if (clip == null) return;
        uiSource.pitch = pitch;
        uiSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null || musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlayHitSound(string entityTypeID)
    {
        AudioClip clip = audioDB.GetHitSound(entityTypeID);
        if (clip != null)
        {
            sfxSource.pitch = Random.Range(0.9f, 1.1f);
            sfxSource.PlayOneShot(clip);
        }
    }
}