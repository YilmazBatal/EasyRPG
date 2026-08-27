using System.Collections.Generic;
using TextBasedRPG.Core.Items;
using UnityEngine;
using UnityEngine.UI;

public class BlacksmithManager : MonoBehaviour
{
    #region Inspector References
    [Header("Filter Buttons (Weapon/Armor)")]
    [SerializeField] private Button weaponFilter;
    [SerializeField] private Button armorFilter;
    [Range(0f, 1f)][SerializeField] private float inactiveTabAlpha = 0.35f;
    [SerializeField] private Transform selectionsParent; // this is where the selection cards will be instantiated

    [Header("Prefab")]
    [SerializeField] private GameObject equipmentSelectionCard;
    #endregion

    private ItemType _currentCategory = ItemType.Weapon;
    private GameContext _context;

    private void Start()
    {
        _context = GameManager.Instance.Context;

        if (_context == null)
        {
            Debug.LogError("[BlacksmithManager] GameManager.Instance.Context is null!");
            return;
        }

        weaponFilter?.onClick.AddListener(() => SwitchCategory(ItemType.Weapon));
        armorFilter?.onClick.AddListener(() => SwitchCategory(ItemType.Armor));


        SwitchCategory(ItemType.Weapon);
    }

    public void SwitchCategory(ItemType category)
    {
        _currentCategory = category;
        RefreshItems();
    }

    private static void SetImageAlpha(Image img, float alpha)
    {
        if (img == null) return;
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    private void RefreshItems()
    {
        ClearContent();


    }

    private void ClearContent()
    {
        if (contentParent == null) return;

        for (int i = contentParent.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.GetChild(i).gameObject);
        }
    }
}
