using System.Collections.Generic;
using UnityEngine;
using Assets._Project.Scripts.Enums;
using TMPro;

namespace Assets._Project.Scripts.ScriptableObjects.ScriptableObjectScripts
{
    [System.Serializable]
    public struct RarityStyle
    {
        public Rarity rarity;
        public Color color;
        public TMP_ColorGradient gradient; 
    }

    [CreateAssetMenu(fileName = "RarityDatabase", menuName = "Data/Rarity Database")]
    public class RarityDatabase : ScriptableObject
    {
        [SerializeField] private List<RarityStyle> styles;

        private Dictionary<Rarity, RarityStyle> _styleDict;

        private void OnEnable()
        {
            _styleDict = new Dictionary<Rarity, RarityStyle>();
            foreach (var style in styles)
            {
                if (!_styleDict.ContainsKey(style.rarity))
                    _styleDict.Add(style.rarity, style);
            }
        }

        /// <summary>
        /// Gets basic color for the rarity
        /// </summary>
        /// <param name="rarity"></param>
        /// <returns>Color of the rarity (Color)</returns>
        public Color GetColor(Rarity rarity)
        {
            if (_styleDict.TryGetValue(rarity, out var style))
                return style.color;

            return Color.white; // Fallback
        }
        /// <summary>
        /// Gets the TMP_ColorGradient for the titles.
        /// </summary>
        /// <param name="rarity"></param>
        /// <returns></returns>
        public TMP_ColorGradient GetGradient(Rarity rarity)
        {
            if (_styleDict.TryGetValue(rarity, out var style))
                return style.gradient;

            return _styleDict[Rarity.Common].gradient; // Fallback
        }
    }
}
