using Assets._Project.Scripts.Enums;
using Assets._Project.Scripts.ScriptableObjects.ScriptableObjectScripts;
using TextBasedRPG.Core.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.UI.Cards.RightSection
{
    [System.Serializable]
    public class EquipmentSlotUI
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text tagText;

        public void UpdateSlot(Item item, RarityDatabase rarityDB)
        {
            if (item != null)
                SetData(item.Name, item.Rarity, rarityDB);
            else
                SetData("None", Rarity.Common, rarityDB);
        }

        private void SetData(string name, Rarity rarity, RarityDatabase rarityDB)
        {
            Color rarityColor = rarityDB.GetColor(rarity);

            icon.color = rarityColor;

            tagText.text = rarity.ToString();
            tagText.color = rarityColor;

            nameText.text = name;
            nameText.enableVertexGradient = true; 
            nameText.colorGradientPreset = rarityDB.GetGradient(rarity);
        }
    }
}
