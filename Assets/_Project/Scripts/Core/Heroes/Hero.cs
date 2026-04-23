using System;
using System.Collections.Generic;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Events;
using TextBasedRPG.Models;

namespace TextBasedRPG.Core.Heroes
{
    public abstract class Hero
    {
        #region Basic Info & Base Stats
        public string? ClassName { get; set; }
        public string? Description { get; set; }
        public int BaseHP { get; protected set; }
        public int BaseATK { get; protected set; }
        public int BaseDEF { get; protected set; }
        public int BaseSPD { get; protected set; }
        public string? ActiveLocation { get; set; }
        public int? UnlockedUntill { get; set; }
        #endregion
        #region Inventory and Equipments
        public List<InventoryData>? Inventory {  get; set; } = new List<InventoryData>();
        public Weapon? EquippedWeapon { get; set; }
        public Armor? EquippedArmor { get; set; }
        #endregion
        #region Progression
        public int Level { get; internal set; } = 1;
        public int CurExp { get; internal set; } = 0; 
        public int TotalExp { get; internal set; } = 0;
        public int ReqExp => (int)(100 * Math.Pow(Level, 1.5));
        #endregion
        #region Stat Points / Training
        public int UnusedStatPoints { get; internal set; } = 0;
        public int InvestedSTRPoints { get; internal set; } = 0;
        public int InvestedVITPoints { get; internal set; } = 0;
        public int InvestedDEXPoints { get; internal set; } = 0;
        public int InvestedAGIPoints { get; internal set; } = 0;
        #endregion
        #region Currency
        public int Gold { get; internal set; } = 0;
        #endregion
        #region Combat stats             
        public int BonusATK { get; internal set; }
        public int BonusDEF { get; internal set; }
        public int BonusSPD { get; internal set; }
        public float BonusCritRate { get; internal set; } // %
        public float BonusCritDMG { get; internal set; } // %
        public int BonuslessATK => BaseATK + (int)Math.Round((EquippedWeapon?.WeaponATK ?? 5) * (1f + (EquippedWeapon?.Upgrade ?? 0) * 0.05f)) + (int)Math.Round(InvestedSTRPoints * 1.5);
        public int BonuslessDEF => BaseDEF + (int)Math.Round((EquippedArmor?.ArmorDef ?? 5) * (1f + (EquippedArmor?.Upgrade ?? 0) * 0.05f)) + (int)Math.Round(InvestedVITPoints * 1.5);
        public int BonuslessSPD => BaseSPD + (int)Math.Round(InvestedAGIPoints * 1.5);
        public float BonuslessCritRate => 5f + (InvestedDEXPoints * 1.0f / 3.0f);
        public float BonuslessCritDamage => 150f + (InvestedSTRPoints);

        public int TotalATK => BonuslessATK + BonusATK;
        public int TotalDEF => BonuslessDEF + BonusDEF;
        public int TotalSPD => BonuslessSPD + BonusSPD;
        public int TotalHP => BaseHP + (EquippedArmor?.ExtraHP ?? 0) + (int)Math.Round(InvestedVITPoints * 1.5);
        public int CurHP { get; internal set; } = 100;
        public float CritRate => BonuslessCritRate + BonusCritRate; // %
        public float CritDamage => BonuslessCritDamage + BonusCritDMG; // %
        public float EvasionRate => 5f + InvestedAGIPoints * 0.2f;
        // Other
        public int Deaths { get; internal set; } = 0;
        public int EntitiesSlayed { get; internal set; } = 0;
        public int HeaviestDamage { get; internal set; } = 0;
        #endregion
        #region Methods
        public void UpdateHeaviestDamage(int damage)
        {
            if (damage > HeaviestDamage)
                HeaviestDamage = damage;
            GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);
        }

        public void EquipItem(Item item)
        {
            if (item is Weapon w)
            {
                if (EquippedWeapon == w)
                {
                    // Unequip
                    Inventory.Add(new InventoryData
                    {
                        InstanceID = System.Guid.NewGuid().ToString(),
                        ID = EquippedWeapon.ID,
                        Upgrade = EquippedWeapon.Upgrade,
                        Quantity = 1
                    });
                    EquippedWeapon = null;
                }
                else
                {
                    // Equip new weapon, first unequip current if any
                    if (EquippedWeapon != null)
                    {
                        Inventory.Add(new InventoryData
                        {
                            InstanceID = System.Guid.NewGuid().ToString(),
                            ID = EquippedWeapon.ID,
                            Upgrade = EquippedWeapon.Upgrade,
                            Quantity = 1
                        });
                    }
                    EquippedWeapon = w;

                    // Remove the equipped weapon from inventory
                    for (int i = 0; i < Inventory.Count; i++)
                    {
                        if (Inventory[i].ID == w.ID && Inventory[i].Upgrade == w.Upgrade)
                        {
                            Inventory.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            else if (item is Armor a)
            {
                if (EquippedArmor == a)
                {
                    // Unequip
                    Inventory.Add(new InventoryData
                    {
                        InstanceID = System.Guid.NewGuid().ToString(),
                        ID = EquippedArmor.ID,
                        Upgrade = EquippedArmor.Upgrade,
                        Quantity = 1
                    });
                    EquippedArmor = null;
                }
                else
                {
                    // Equip new armor, first unequip current if any
                    if (EquippedArmor != null)
                    {
                        Inventory.Add(new InventoryData
                        {
                            InstanceID = System.Guid.NewGuid().ToString(),
                            ID = EquippedArmor.ID,
                            Upgrade = EquippedArmor.Upgrade,
                            Quantity = 1
                        });
                    }
                    EquippedArmor = a;

                    // Remove the equipped armor from inventory
                    for (int i = 0; i < Inventory.Count; i++)
                    {
                        if (Inventory[i].ID == a.ID && Inventory[i].Upgrade == a.Upgrade)
                        {
                            Inventory.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);
            EventManager.HeroEvents.TriggerEquipmentChanged(GameManager.Instance.Context);
        }
        public void ConsumeItem()
        {
            GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);
        }

        public void FullHeal()
        {
            CurHP = TotalHP;
            GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);

        }

        public void ApplyDeathPenalty()
        {
            int goldPenalty = (int)(Gold * 0.10);
            Gold -= goldPenalty;

            int expPenalty = (int)(CurExp * 0.33);
            CurExp -= expPenalty;

            if (CurExp < 0) CurExp = 0;

            FullHeal();

            EventManager.HeroEvents.TriggerHPValueChanged(GameManager.Instance.Context);
            EventManager.HeroEvents.TriggerGoldChanged(GameManager.Instance.Context);
            EventManager.HeroEvents.TriggerExpChanged(GameManager.Instance.Context);
            
            GameManager.Instance.SaveService.SaveGame(GameManager.Instance.Context);
            Toaster.Instance.ShowToast($"You have died and suffered penalties. Lost Gold: {goldPenalty}, Lost Experience: {expPenalty}", UIManager.Instance.IconDB.boneIcon);
        }
        #endregion
    }
}
