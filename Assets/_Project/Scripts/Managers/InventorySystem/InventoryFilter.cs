using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TextBasedRPG.Core.Items;
using Material = TextBasedRPG.Core.Items.Material;

namespace TextBasedRPG.Managers.Inventory
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class InventoryFilter : MonoBehaviour
    {
        [Header("Filter Toggles")]
        [SerializeField] Toggle filterWeapons;
        [SerializeField] Toggle filterArmors;
        [SerializeField] Toggle filterConsumables;
        [SerializeField] Toggle filterMaterials;
        [SerializeField] Toggle filterMix;

        [Header("Visuals")]
        [Range(0f, 1f)]
        [SerializeField] float offAlpha = 0.66f;
        [Header("Debug")]
        [SerializeField] bool enableDebugLogs = false;

        public UnityEvent onFilterChanged = new UnityEvent();

        UnityAction<bool> weaponsListener;
        UnityAction<bool> armorsListener;
        UnityAction<bool> consumablesListener;
        UnityAction<bool> materialsListener;
        UnityAction<bool> mixListener;

        private void OnEnable()
        {
            filterWeapons.isOn = true;
            filterArmors.isOn = true;
            filterConsumables.isOn = true;
            filterMaterials.isOn = true;
            filterMix.isOn = true;

            if (filterWeapons != null)
            {
                weaponsListener = val => { UpdateToggleVisual(filterWeapons); NotifyChanged("Weapons", val); };
                filterWeapons.onValueChanged.AddListener(weaponsListener);
                UpdateToggleVisual(filterWeapons);
            }

            if (filterArmors != null)
            {
                armorsListener = val => { UpdateToggleVisual(filterArmors); NotifyChanged("Armors", val); };
                filterArmors.onValueChanged.AddListener(armorsListener);
                UpdateToggleVisual(filterArmors);
            }

            if (filterConsumables != null)
            {
                consumablesListener = val => { UpdateToggleVisual(filterConsumables); NotifyChanged("Consumables", val); };
                filterConsumables.onValueChanged.AddListener(consumablesListener);
                UpdateToggleVisual(filterConsumables);
            }

            if (filterMaterials != null)
            {
                materialsListener = val => { UpdateToggleVisual(filterMaterials); NotifyChanged("Materials", val); };
                filterMaterials.onValueChanged.AddListener(materialsListener);
                UpdateToggleVisual(filterMaterials);
            }

            if (filterMix != null)
            {
                mixListener = val => { UpdateToggleVisual(filterMix); NotifyChanged("Mix", val); };
                filterMix.onValueChanged.AddListener(mixListener);
                UpdateToggleVisual(filterMix);
            }
        }

        private void OnDisable()
        {
            if (filterWeapons != null && weaponsListener != null) filterWeapons.onValueChanged.RemoveListener(weaponsListener);
            if (filterArmors != null && armorsListener != null) filterArmors.onValueChanged.RemoveListener(armorsListener);
            if (filterConsumables != null && consumablesListener != null) filterConsumables.onValueChanged.RemoveListener(consumablesListener);
            if (filterMaterials != null && materialsListener != null) filterMaterials.onValueChanged.RemoveListener(materialsListener);
            if (filterMix != null && mixListener != null) filterMix.onValueChanged.RemoveListener(mixListener);
        }

        void UpdateToggleVisual(Toggle t)
        {
            if (t == null) return;
            CanvasGroup cg = t.GetComponent<CanvasGroup>();
            if (cg == null) cg = t.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = t.isOn ? 1f : offAlpha;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        void NotifyChanged(string name, bool value)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"InventoryFilter: Toggle '{name}' changed -> {value}");
            }
            onFilterChanged?.Invoke();
        }

        public bool AnyFilterActive
        {
            get
            {
                return (filterWeapons != null && filterWeapons.isOn) ||
                       (filterArmors != null && filterArmors.isOn) ||
                       (filterConsumables != null && filterConsumables.isOn) ||
                       (filterMaterials != null && filterMaterials.isOn) ||
                       (filterMix != null && filterMix.isOn);
            }
        }

        public bool ShouldShowItem(Item templateItem)
        {
            if (!AnyFilterActive) return true;

            if (templateItem is Weapon)
                return filterWeapons != null && filterWeapons.isOn;

            if (templateItem is Armor)
                return filterArmors != null && filterArmors.isOn;

            if (templateItem is Material)
                return filterMaterials != null && filterMaterials.isOn;

            if (templateItem is Consumable)
                return filterConsumables != null && filterConsumables.isOn;

            return filterMix != null && filterMix.isOn;
        }
    }
}
