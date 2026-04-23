using Assets._Project.Scripts.UI.Cards;
using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Managers.Inventory;
using TextBasedRPG.Models;
using TextBasedRPG.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Material = TextBasedRPG.Core.Items.Material;

public class BackpackManager : MonoBehaviour
{
    #region Variables
    [Header("Inventory & Item Card Prefab")]
    [SerializeField] TMP_Text inventoryCapacityText;
    [SerializeField] GameObject inventoryContent;
    [SerializeField] GameObject itemCardPrefab;
    [SerializeField] GameObject detailsCardPrefab;
    [SerializeField] InventoryFilter inventoryFilter;
    
    GameContext context;
    IconDatabase iconDB => UIManager.Instance.IconDB;
    #endregion

    private void OnEnable()
    {
        EventManager.HeroEvents.OnEquipmentChanged += OnEquipmentChanged;

        context = GameManager.Instance.Context;
        if (inventoryFilter != null)
            inventoryFilter.onFilterChanged.AddListener(GenerateItemCards);

        GenerateItemCards();

        inventoryCapacityText.text = $"{context.Player.Inventory.Count}/20"; // hard coded 20 for now
    }
    private void OnDisable()
    {
        EventManager.HeroEvents.OnEquipmentChanged -= OnEquipmentChanged;

        if (inventoryFilter != null)
        {
            inventoryFilter.onFilterChanged.RemoveListener(GenerateItemCards);
        }
    }
    public void GenerateItemCards()
    {
        ClearItems(); // Clear to overcome dumplication.

        GenerateEquippedItemsCards();
        GenerateInventoryItemsCards();
    }
    private void GenerateInventoryItemsCards()
    {
        foreach (var item in context.Player.Inventory)
        {
            Item masterItem = GetItemWithID(item.ID); // Get base item
            if (masterItem != null)
            {
                if (inventoryFilter != null && !inventoryFilter.ShouldShowItem(masterItem)) continue;

                Item newItem = masterItem.Clone();

                if (newItem is Material || newItem is Consumable) newItem.Quantity = item.Quantity;
                if (newItem is Weapon w) w.Upgrade = item.Upgrade;
                if (newItem is Armor a) a.Upgrade = item.Upgrade;

                GameObject newCard = Instantiate(itemCardPrefab, inventoryContent.transform);
                InventoryCard card = newCard.GetComponent<InventoryCard>();

                card.ModifyItemCard(newItem); // Modify the item card with the item data and append to inventory content
            }
        }
    }
    private void GenerateEquippedItemsCards()
    {
        Weapon equippedWeapon = context.Player.EquippedWeapon;
        Armor equippedArmor = context.Player.EquippedArmor;

        // Only show equipped items if there is no filter or the filter allows them
        if (equippedWeapon != null && (inventoryFilter == null || inventoryFilter.ShouldShowItem(equippedWeapon)))
        {
            GameObject newCard = Instantiate(itemCardPrefab, inventoryContent.transform);
            InventoryCard card = newCard.GetComponent<InventoryCard>();
            
            card.ModifyItemCard(equippedWeapon, true);
        }


        if (equippedArmor != null && (inventoryFilter == null || inventoryFilter.ShouldShowItem(equippedArmor)))
        {
            GameObject newCard2 = Instantiate(itemCardPrefab, inventoryContent.transform);
            InventoryCard card2 = newCard2.GetComponent<InventoryCard>();
            
            card2.ModifyItemCard(equippedArmor, true);
        }
    }
    private Item GetItemWithID(string ID)
    {
        if (context.MasterItemBook.TryGetValue(ID, out var itemData))
        {
            return itemData;
        }
        else
        {
            Debug.LogWarning($"Item with ID {ID} not found in MasterItemBook.");
            return null;
        }
    }
    private void ClearItems()
    {
        foreach (Transform child in inventoryContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    #region Events
    private void OnEquipmentChanged(GameContext context) => GenerateItemCards();
    #endregion
}
