using System;
using System.Collections.Generic;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Interfaces;
using TextBasedRPG.Managers;
using TextBasedRPG.Models;
using Random = UnityEngine.Random;

namespace TextBasedRPG.Core.Heroes
{
    public abstract class Hero/* : IDamageable*/
    {
        public List<InventoryData>? Inventory {  get; set; } = new List<InventoryData>();
        // Basic Info
        public string? ClassName { get; set; }
        public string? Description { get; set; }
        public string? ActiveLocation { get; set; }
        public int? UnlockedUntill { get; set; }
        // Equipments 
        public Weapon? EquippedWeapon { get; set; }
        public Armor? EquippedArmor { get; set; }
        // Base Stats
        public int BaseHP { get; protected set; }
        public int BaseATK { get; protected set; }
        public int BaseDEF { get; protected set; }
        public int BaseSPD { get; protected set; }
        // Experience and Level
        public int Level { get; internal set; } = 1;
        public int CurExp { get; internal set; } = 0; 
        public int ReqExp => (int)(100 * Math.Pow(Level, 1.5));
        // Stat points
        public int UnusedStatPoints { get; internal set; } = 0;
        public int InvestedSTRPoints { get; internal set; } = 0;
        public int InvestedVITPoints { get; internal set; } = 0;
        public int InvestedDEXPoints { get; internal set; } = 0;
        public int InvestedAGIPoints { get; internal set; } = 0;
        // Currency
        public int Gold { get; internal set; } = 0;
        // Advanced stats             
        public int BonusATK { get; internal set; }
        public int BonusDef { get; internal set; }
        public int TotalATK => BaseATK + (EquippedWeapon?.WeaponATK ?? 5) + (int)Math.Round(InvestedSTRPoints * 1.5) + BonusATK;
        public int TotalDEF => BaseDEF + (EquippedArmor?.ArmorDef ?? 0) + (int)Math.Round(InvestedVITPoints * 1.5) + BonusDef;
        public int TotalHP => BaseHP + (EquippedArmor?.ExtraHP ?? 0) + (int)Math.Round(InvestedVITPoints * 1.5);
        public int TotalSPD => BaseSPD + (int)Math.Round(InvestedAGIPoints * 1.5);
        public int CurHP { get; internal set; } = 100;
        public float CritRate => 5f + InvestedDEXPoints * 1.0f / 3.0f; // %
        public float CritDamage => 150f + InvestedSTRPoints; // %
        public float EvasionRate => 5f + InvestedAGIPoints * 1.0f / 3.0f; 

        /// <summary>
        /// Method to display hero's base stats summary in choosing screen 
        /// </summary>
        public void GetStatsSummary()
        {
        //    Console.WriteLine($"""
                
        //        =========================
        //        === {ClassName} Stats ===
        //        BASE HP  : {BaseHP}
        //        BASE ATK : {BaseATK}
        //        BASE DEF : {BaseDEF}
        //        -----------------------
        //        Desc: {Description}
        //        =========================

        //        """);
        }

        //public void TakeDamage(int amount)
        //{
        //    bool didEvade = Random.Range(0, 101) <= EvasionRate;
        //    if (didEvade) {
        //        //Console.WriteLine("User has dodged the attack.");
        //    }
        //    else {
        //        CurHP -= Math.Max(1, amount);
        //        if (CurHP < 0) CurHP = 0;
        //    }

        //}
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

            CurHP = TotalHP;
            
            // Spawn penalty notification somewhere 
            //MenuUI.ColoredMsg(ConsoleColor.Red, "\n[DEATH] You have died and suffered penalties.");
            //MenuUI.ColoredMsg(ConsoleColor.Yellow, $"[PENALTY] Lost Gold: {goldPenalty}");
            //MenuUI.ColoredMsg(ConsoleColor.Cyan, $"[PENALTY] Lost Experience: {expPenalty}");
            //MenuUI.ColoredMsg(ConsoleColor.Yellow, $"[SYSTEM] You will be resurrected.");
        }
    }
}
