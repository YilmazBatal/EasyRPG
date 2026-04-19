using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.UI.Cards
{
    public class DetailsCard : MonoBehaviour
    {
        [SerializeField] public Image background;

        [SerializeField] public TMP_Text title;
        [SerializeField] public TMP_Text desc;
        [SerializeField] public Image itemIcon;

        [SerializeField] public Image icon1;
        [SerializeField] public TMP_Text value1;
        [SerializeField] public Image icon2;
        [SerializeField] public TMP_Text value2;
        [SerializeField] public Image icon3;
        [SerializeField] public TMP_Text value3;
        [SerializeField] public Image icon4;
        [SerializeField] public TMP_Text value4;
        [SerializeField] public Image icon5;
        [SerializeField] public TMP_Text value5;

        [SerializeField] public TMP_Text gold;

        public Button action;
        public Button closeBTN;

        private void OnEnable()
        {
            closeBTN.onClick.RemoveAllListeners();
            closeBTN.onClick.AddListener(() => CloseDetailsMenu(background.gameObject));
        }
        private void CloseDetailsMenu(GameObject panel)
        {
            closeBTN.interactable = false;

            LeanTween.value(panel.gameObject, 1f, 0f, 0.3f).setEaseInOutCubic().setOnUpdate((float val) =>
            {
                panel.transform.localScale = new Vector3(val, val, val);
            }).setOnComplete(() =>
            {
                GameObject shadow = panel.gameObject.transform.parent.gameObject;
                Image image = shadow.GetComponent<Image>();
                LeanTween.value(shadow, image.color.a, 0f, 0.1f).setEaseLinear().setOnUpdate((float val) =>
                {
                    shadow.GetComponent<Image>().color = new Color(0f, 0f, 0f, val);
                }).setOnComplete(() => Destroy(shadow.gameObject));
            });
        }

    }
}
