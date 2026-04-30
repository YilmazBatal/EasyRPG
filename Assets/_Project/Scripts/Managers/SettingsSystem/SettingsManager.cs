using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    #region Components
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;
    #endregion

    void OnEnable()
    {
        masterSlider.onValueChanged.AddListener((val) => AudioManager.Instance.UpdateMixerVolume("MasterVol", val));
        musicSlider.onValueChanged.AddListener((val) => AudioManager.Instance.UpdateMixerVolume("MusicVol", val));
        sfxSlider.onValueChanged.AddListener((val) => AudioManager.Instance.UpdateMixerVolume("SFXVol", val));
        uiSlider.onValueChanged.AddListener((val) => AudioManager.Instance.UpdateMixerVolume("UIVol", val));
    }
}
