using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Toaster : MonoBehaviour
{
    public static Toaster Instance;
    #region Variables
    [SerializeField] CanvasGroup cg;
    [SerializeField] TMP_Text toastText;
    [SerializeField] Image toastIcon;
    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
        cg.gameObject.SetActive(false);
    }
    public void ShowToast(string text, Sprite icon)
    {
        LeanTween.cancel(cg.gameObject);
        cg.gameObject.SetActive(true);
        Instance.toastText.text = text;
        Instance.toastIcon.sprite = icon;
        LeanTween.value(0, 1, 0.5f).setOnUpdate((float val) =>
        {
            Instance.cg.alpha = val;
        }).setOnComplete(() =>
        {
            LeanTween.value(1, 0, 0.5f).setDelay(2f).setOnUpdate((float val) =>
            {
                Instance.cg.alpha = val;
            }).setOnComplete(() =>
            {
                Instance.cg.gameObject.SetActive(false);
                Instance.toastText.text = string.Empty;
                Instance.toastIcon.sprite = UIManager.Instance.IconDB.questionMarkIcon;
            });
        });
        
    }
}
