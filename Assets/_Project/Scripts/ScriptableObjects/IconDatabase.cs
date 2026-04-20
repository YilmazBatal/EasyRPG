using UnityEngine;

[CreateAssetMenu(fileName = "IconDatabase", menuName = "UI/Icon Database")]
public class IconDatabase : ScriptableObject
{
    [Header("Equipment Icons")]
    public Sprite fistIcon;
    public Sprite swordIcon;
    public Sprite bowIcon;
    public Sprite staffIcon;
    public Sprite armorIcon;

    [Header("Item Icons")]
    public Sprite boneIcon;
    public Sprite meatIcon;
    public Sprite potIcon;

    [Header("Other Icons")]
    public Sprite attackIcon;
    public Sprite levelIcon;
    public Sprite upgradeIcon;
    public Sprite hpIcon;
    public Sprite quantityIcon;
    public Sprite expIcon;
    public Sprite luckIcon;
    public Sprite speedIcon;
    public Sprite featherIcon;
    public Sprite appleIcon;
    public Sprite craftIcon;
    public Sprite goldIcon;
    public Sprite hammerIcon;
    public Sprite keyIcon;
    public Sprite plusIcon;
    public Sprite questionMarkIcon;
    public Sprite questIcon;

    public Sprite GetWeaponIcon(WeaponType type) => type switch
    {
        WeaponType.Sword => swordIcon,
        WeaponType.Bow => bowIcon,
        WeaponType.Staff => staffIcon,
        _ => questionMarkIcon
    };
}