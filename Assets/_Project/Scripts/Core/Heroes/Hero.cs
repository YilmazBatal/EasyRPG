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
        public int BonuslessATK => BaseATK + (EquippedWeapon?.WeaponATK ?? 5) +(int)Math.Round(InvestedSTRPoints * 1.5);
        public int BonuslessDEF => BaseDEF + (EquippedArmor?.ArmorDef ?? 5) +(int)Math.Round(InvestedVITPoints * 1.5);
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
        public float EvasionRate => 5f + InvestedAGIPoints * 1.0f / 3.0f;
        // Other
        public int Deaths { get; internal set; } = 0;
        public int EntitiesSlayed { get; internal set; } = 0;
        public int HeaviestDamage { get; internal set; } = 0;
        #endregion
        #region Methods
        public void UpdateHeaviestDamage(int damage)
        {
            if (damage > HeaviestDamage)
            {
                HeaviestDamage = damage;
            }
        }

        public void FullHeal()
        {
            CurHP = TotalHP;
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

            // Spawn penalty notification somewhere 
            //MenuUI.ColoredMsg(ConsoleColor.Red, "\n[DEATH] You have died and suffered penalties.");
            //MenuUI.ColoredMsg(ConsoleColor.Yellow, $"[PENALTY] Lost Gold: {goldPenalty}");
            //MenuUI.ColoredMsg(ConsoleColor.Cyan, $"[PENALTY] Lost Experience: {expPenalty}");
            //MenuUI.ColoredMsg(ConsoleColor.Yellow, $"[SYSTEM] You will be resurrected.");
        }
        #endregion
    }
}
