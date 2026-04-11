using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.UI
{
    public static class UIExtensions
    {
        public static void Shake(RectTransform rect, float intensity, float duration)
        {
            Vector2 originalPos = rect.anchoredPosition;

            LeanTween.cancel(rect.gameObject);

            LeanTween.value(rect.gameObject, intensity, 0f, duration)
                .setOnUpdate((float v) => rect.anchoredPosition = originalPos + Random.insideUnitCircle * v)
                .setOnComplete(() => rect.anchoredPosition = originalPos);
        }
        public static void Flash(Image avatar)
        {
            LeanTween.cancel(avatar.gameObject);

            LeanTween.value(avatar.gameObject, 1f, 0f, 0.15f)
                .setEaseInOutCubic()
                .setLoopPingPong(1)
                .setOnUpdate((float val) =>
                {
                    avatar.color = new Color(
                        avatar.color.r,
                        avatar.color.g,
                        avatar.color.b,
                        val);
                });
        }
        public static void GenerateDamageText(TMP_Text tmp, bool isCrit, int damage)
        {
            CanvasGroup cg = tmp.GetComponent<CanvasGroup>();
            RectTransform textRect = cg.gameObject.GetComponent<RectTransform>();
            Vector2 anchoredPos = cg.transform.position;
            Vector2 startAnchoredPos = textRect.anchoredPosition;

            tmp.text = damage.ToString();

            LeanTween.cancel(cg.gameObject);

            LeanTween.value(cg.gameObject, 0f, 1f, 0.5f)
                .setEaseInOutCubic()
                .setLoopPingPong(1)
                .setOnUpdate((float val) =>
                {
                    cg.alpha = val;
                });

            LeanTween.value(cg.gameObject, 0f, 50f, 0.5f)
                .setEaseInOutCubic()
                .setLoopPingPong(1)
                .setOnUpdate((float val) =>
                {
                    textRect.anchoredPosition = new Vector2(startAnchoredPos.x, startAnchoredPos.y + val);
                }).setOnComplete(() =>
                {
                    textRect.anchoredPosition = startAnchoredPos;
                });
        }
        public static void GhostBarFill(Image bar, Image ghostBar, float targetFill)
        {
            float currentFill = bar.fillAmount;

            LeanTween.cancel(ghostBar.gameObject);
            LeanTween.cancel(bar.gameObject);

            if (targetFill > currentFill)
            {
                LeanTween.value(ghostBar.gameObject, ghostBar.fillAmount, targetFill, 0.2f)
                    .setEaseOutQuad()
                    .setOnUpdate((float val) => ghostBar.fillAmount = val);

                LeanTween.value(bar.gameObject, bar.fillAmount, targetFill, 0.6f)
                    .setDelay(0.2f)
                    .setEaseInOutCubic()
                    .setOnUpdate((float val) => bar.fillAmount = val);
            }
            else
            {
                LeanTween.value(bar.gameObject, bar.fillAmount, targetFill, 0.2f)
                    .setEaseOutQuad()
                    .setOnUpdate((float val) => bar.fillAmount = val);

                LeanTween.value(ghostBar.gameObject, ghostBar.fillAmount, targetFill, 0.6f)
                    .setDelay(0.2f)
                    .setEaseInOutCubic()
                    .setOnUpdate((float val) => ghostBar.fillAmount = val);
            }
        }
    }
}
