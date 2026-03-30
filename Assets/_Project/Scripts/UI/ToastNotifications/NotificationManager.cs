using TMPro;
using UnityEngine;

namespace Assets._Project.Scripts.UI.ToastNotifications
{
    public class NotificationManager : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] TMP_Text expText;
        [SerializeField] TMP_Text goldText;
        private void Awake()
        {
            canvasGroup.alpha = 0;
            expText.text = "Null";
            goldText.text = "Null";
        }
        public void UpdateNotification(string exp, string gold)
        {
            expText.text = exp;
            goldText.text = gold;

            LeanTween.alphaCanvas(canvasGroup, 1f, 0.3f).setEaseLinear().setOnComplete(() =>
            {
                LeanTween.alphaCanvas(canvasGroup, 0f, 0.3f).setDelay(2f).setEaseLinear();
            });
        }
    }
}
