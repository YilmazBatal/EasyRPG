using Assets._Project.Scripts.Enums;
using Assets._Project.Scripts.ScriptableObjects.ScriptableObjectScripts;
using TextBasedRPG.Core.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.UI.Cards.RightSection
{
    [System.Serializable]
    public class EquipmentSlotUI : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject detailsCardPrefab;

        [SerializeField] private Outline rarityOutline;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text tagText;

        [SerializeField] private Button detailsButton;
        [SerializeField] private Button changeButton;

        public void UpdateSlot(Item item, RarityDatabase rarityDB)
        {
            if (item != null)
                SetData(item.Name, item.Rarity, rarityDB, item);
            else
                SetData("None", Rarity.Common, rarityDB, null);
        }

        private void SetData(string name, Rarity rarity, RarityDatabase rarityDB, Item item)
        {
            Color rarityColor = rarityDB.GetColor(rarity);

            rarityOutline.effectColor = rarityColor;
            icon.color = rarityColor;

            tagText.text = rarity.ToString();
            tagText.color = rarityColor;

            nameText.text = name;
            nameText.enableVertexGradient = true; 
            nameText.colorGradientPreset = rarityDB.GetGradient(rarity);

            detailsButton.onClick.RemoveAllListeners();
            detailsButton.onClick.AddListener(() => DetailsButton(item, null));
        }

        // HERE MAKE A DETAILS CARD POPUP FOR THE ITEM LATER ONLY AND ONLY AFTER THE CORE LOOP / MVP.
        private void DetailsButton(Item item, InventoryCard inventoryCard)
        {
            Toaster.Instance.ShowToast("Item details coming soon!", UIManager.Instance.IconDB.lockedIcon);
            //if (detailsCardPrefab == null)
            //{
            //    Debug.LogError("detailsCardPrefab is missing! Please assign it in the inspector.");
            //    return;
            //}



            //ill fix this funny transform later.
            //GameObject detailsObj = Instantiate(detailsCardPrefab, transform.parent.transform.parent.transform.parent.transform.parent.transform);

            //detailsObj.transform.GetChild(0).GetComponent<DetailsCard>().OpenDetailsMenu(detailsObj, item, inventoryCard);
        }
    }
}
