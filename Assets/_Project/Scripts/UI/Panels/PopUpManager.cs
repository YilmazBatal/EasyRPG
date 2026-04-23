using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    [SerializeField] public TMP_Text title;
    [SerializeField] public TMP_Text description;
    [SerializeField] public Button cancel;
    [SerializeField] public Button confirm;

    public void Close()
    {
        LeanTween.value(UIManager.Instance.raycastBlocker1.gameObject, 0.5f, 0f, 0.2f).setEaseInOutCubic().setOnUpdate((float val) =>
        {
            UIManager.Instance.raycastBlocker1.color = new Color(0f, 0f, 0f, val);
        });
        UIManager.Instance.raycastBlocker1.gameObject.SetActive(false);

        LeanTween.value(this.gameObject, 1f, 0f, 0.3f).setEaseInOutCubic().setOnUpdate((float val) =>
        {
            this.transform.localScale = new Vector3(val, val, val);
        }).setOnComplete(() => {
           Destroy(this.gameObject);

        });
    }
}
