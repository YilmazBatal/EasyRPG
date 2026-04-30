using UnityEngine;
using UnityEngine.UI;

public class GlobalUIClicker : MonoBehaviour
{
    [SerializeField] private AudioClip defaultClickSound;

    void Start()
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button btn in allButtons)
        {
            btn.onClick.AddListener(() => PlayDefaultClick());
        }
    }

    private void PlayDefaultClick()
    {
        if (AudioManager.Instance != null && defaultClickSound != null)
        {
            AudioManager.Instance.PlayUI(defaultClickSound);
        }
    }
}