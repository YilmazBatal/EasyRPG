using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;
    
    [SerializeField] CanvasGroup cg;
    [SerializeField] Image loadingBar;
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] TextMeshProUGUI infoText;

    [SerializeField] private string[] infoMessages;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        loadingBar.fillAmount = 0f;
        infoText.text = infoMessages[0];
        StartCoroutine(LoadingSequence());
    }

    public IEnumerator UpdateProgressBarCoroutine(float targetFill, float time, string text)
    {
        float currentFill = loadingBar.fillAmount;
        loadingText.text = text;
        LeanTween.cancel(loadingBar.gameObject);

        if (targetFill > currentFill)
        {
            LeanTween.value(loadingBar.gameObject, loadingBar.fillAmount, targetFill, time)
                .setEaseInOutCirc()
                .setOnUpdate((float val) => loadingBar.fillAmount = val);
        }
        yield return new WaitForSeconds(time + 1f);
    }

    private IEnumerator LoadingSequence()
    {
        yield return StartCoroutine(UpdateProgressBarCoroutine(0.22f, 1f, "Loading Database..."));
        yield return StartCoroutine(UpdateProgressBarCoroutine(0.34f, 1f, "Initializing save file..."));
        yield return StartCoroutine(UpdateProgressBarCoroutine(0.54f, 0.9f, "Building the town brick by brick..."));
        yield return StartCoroutine(UpdateProgressBarCoroutine(0.77f, 1.2f, "Getting ready for the adventure..."));
        yield return StartCoroutine(UpdateProgressBarCoroutine(1.0f, 0.5f, "WE ARE SO BACK!"));
        yield return new WaitForSeconds(1f);
        LeanTween.value(cg.gameObject, 1f, 0f, 0.5f)
            .setEaseInOutCubic()
            .setOnUpdate((float val) => cg.alpha = val)
            .setOnComplete(() => Destroy(cg.gameObject));
    }

    float elapsedTime = 0f;
    private void Update()
    {
        elapsedTime += Time.deltaTime;
        while (elapsedTime >= 2f)
        {
            infoText.text = infoMessages[Random.Range(0, infoMessages.Length)];
            elapsedTime = 0f;
        }
    }
}
