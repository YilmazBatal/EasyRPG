using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.UI.Cards
{
    public class HeroCardManager : MonoBehaviour
    {
        [SerializeField] public TMP_Text title;
        [SerializeField] public TMP_Text description;
        [SerializeField] public Image icon;
        [SerializeField] public TMP_Text atk;
        [SerializeField] public TMP_Text def;
        [SerializeField] public TMP_Text hp;
        [SerializeField] public TMP_Text spd;
        [SerializeField] public Button btn;
    }
}
