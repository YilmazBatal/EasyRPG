using UnityEngine;
using UnityEngine.UI;

public class GlobalUIClicker : MonoBehaviour
{
    [SerializeField] private AudioClip defaultClickSound;

    void Start()
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Button btn in allButtons)
        {
            btn.onClick.RemoveListener(PlayDefaultClick);
            btn.onClick.AddListener(PlayDefaultClick);
        }

        Debug.Log($"[UI] Toplam {allButtons.Length} butona ses atandı (Kapalılar dahil).");
    }

    private void PlayDefaultClick()
    {
        if (AudioManager.Instance != null && defaultClickSound != null)
        {
            AudioManager.Instance.PlayUI(defaultClickSound);
        }
    }
}