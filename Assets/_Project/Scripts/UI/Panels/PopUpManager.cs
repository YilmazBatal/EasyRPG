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
        UIManager.Instance.raycastBlocker1.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
