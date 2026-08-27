using Assets._Project.Scripts.ScriptableObjects.ScriptableObjectScripts;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Scripts.Managers.Blacksmith
{
    [System.Serializable]
    public struct StatRow
    {
        public GameObject root;
        public Image icon;
        public TMP_Text labelText;
        public TMP_Text valueOldText;
        public TMP_Text valueNewText;

        public void SetStat(Sprite statIcon, string label, string valueOld, string valueNew)
        {
            root.SetActive(true);
            icon.sprite = statIcon;
            labelText.text = label;
            valueOldText.text = valueOld;
            valueNewText.text = valueNew;
        }

        public void Hide() => root.SetActive(false);
    }
    public class SelectionUpgrade : MonoBehaviour
    {
        // -------- Equipment Info
        [SerializeField] private Image equipmentIcon;
        [SerializeField] private TextMeshProUGUI equipmentName;
        [SerializeField] private Outline equipmentOutlineColor;
        [SerializeField] private TextMeshProUGUI equipmentRarityText;
        // -------- Attributes
        [SerializeField] private StatRow[] statRows;
        // -------- Materials and Cost
        [SerializeField] private TextMeshProUGUI successChance;
        [SerializeField] private TextMeshProUGUI upgradeGoldCost;
        [SerializeField] private Button upgradeBtn;
        // -------- Required Systems
        [SerializeField] private RarityDatabase rarityDB;
        private IconDatabase iconDB;

        private void Start()
        {
            iconDB = UIManager.Instance.IconDB;
        }

    }
}
