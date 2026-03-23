using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Interfaces;
using TextBasedRPG.Managers;
using UnityEngine;

namespace TextBasedRPG.States
{
    public class HeroSelectionState : IMenuState
    {
        private readonly ISaveService _saveService;

        public HeroSelectionState(ISaveService saveService)
        {
            _saveService = saveService;
        }

        public GameState Update(GameContext context)
        {

            //Debug.Log("Here working!! inside");

            //if (context.Player != null)
            //{
            //    Debug.Log("Here working!!");
            //    //Console.Clear();
            //    //MenuUI.ColoredMsg(ConsoleColor.DarkGray, "Please choose a hero to overview Eg. 1");
            //    //Console.WriteLine("=== HERO SELECTION ===");
            //    //Console.WriteLine($"""
            //    // ---------------------
            //    //    [1]. Warrior 
            //    //    [2]. Archer
            //    //    [3]. Mage
            //    // ---------------------
            //    // """);

            //    //string? input = Console.ReadLine();

            //    //Hero? candidate = input switch
            //    //{
            //    //    "1" => new Warrior(),
            //    //    "2" => new Archer(),
            //    //    "3" => new Mage(),
            //    //    _ => null
            //    //};

            //    //if (candidate == null)
            //    //{
            //    //    MenuUI.ColoredMsg(ConsoleColor.Red, "\n[SYSTEM] Invalid choice! Press any key to try again...");
            //    //    Console.ReadKey(true);
            //    //    return GameState.HeroSelection;
            //    //}

            //    //if (ConfirmSelection(candidate))
            //    //{
            //    //    context.Player = candidate;
            //    //    context.Player.ActiveLocation = "L001";
            //    //    candidate.FullHeal();
            //    //    _saveService.SaveGame(context);
            //    //    return GameState.MainMenu;
            //    //}
            //}
            //else
            //{
            //    Debug.Log("Player is null, cannot select hero.");
            //}
            return GameState.HeroSelection;
        }

        //private bool ConfirmSelection(Hero candidate)
        //{
        //    candidate.GetStatsSummary();
        //    Console.WriteLine($"Confirm {candidate.ClassName}? [Y/N]");
        //    return Console.ReadLine()?.ToUpper() == "Y";
        //}
    }
}
