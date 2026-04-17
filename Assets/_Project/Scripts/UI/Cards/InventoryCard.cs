using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCard : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] public TMP_Text itemName;
    [SerializeField] public Outline itemRarity;
    [SerializeField] public Image itemIcon;
    [SerializeField] public Image subStatIcon1;
    [SerializeField] public TMP_Text subStat1;
    [SerializeField] public Image subStatIcon2;
    [SerializeField] public TMP_Text subStat2;
    [SerializeField] public Image subStatIcon3;
    [SerializeField] public TMP_Text subStat3;
    [SerializeField] public TMP_Text price;
    [SerializeField] public Button actionBTN;
    [SerializeField] public TMP_Text actionBTNText;
    [SerializeField] public Button detailsBTN;

    [Header("Icons")]
    [SerializeField] public Sprite attackIcon;
    [SerializeField] public Sprite swordIcon;
    [SerializeField] public Sprite bowIcon;
    [SerializeField] public Sprite staffIcon;
    [SerializeField] public Sprite armorIcon;
    [SerializeField] public Sprite materialIcon;
    [SerializeField] public Sprite quantityIcon;
    [SerializeField] public Sprite upgradeIcon;
    [SerializeField] public Sprite hpIcon;
    [SerializeField] public Sprite levelIcon;
    [SerializeField] public Sprite meatIcon;
    [SerializeField] public Sprite speedIcon;
    [SerializeField] public Sprite questIcon;


}
