
//using UnityEngine;

//namespace TextBasedRPG.States
//{
//    public class MainMenuState : IMenuState
//    {
//        private readonly ISaveService _saveService;

//        public MainMenuState(ISaveService saveService)
//        {
//            _saveService = saveService;
//        }

//        public GameState Update(GameContext context)
//        {
//            if (context.IsAutoSaveOn) _saveService.SaveGame(context);

//            //Console.Clear();

//            if (context.Player == null)
//            {
//                return GameState.HeroSelection;
//            }


//            //CharacterUI.HeroPreview(context);
//            //Console.WriteLine("════════════════ MAIN MENU ════════════════");
//            //MenuUI.MenuOption("0", "Detailed Stats", "Detailed Player Information.");
//            //MenuUI.MenuOption("1", "Backpack", "Browse In Your Inventory.");
//            //MenuUI.MenuOption("2", "BlackSmith", "Upgrade Equipments.");
//            //MenuUI.MenuOption("3", "Training", "Improve Yourself.");
//            //MenuUI.MenuOption("4", "Adventure", "Fight Monsters and Find Loots.");
//            //MenuUI.MenuOption("5", "Region Boss", "Challange The Boss.");
//            //MenuUI.MenuOption("6", "Map", "View Map and Travel.");
//            //MenuUI.MenuOption("7", "Quests", "View Quest Board and Get The DUNGEON KEY.");
//            //MenuUI.MenuOption("A", "Auto Save", $"Toggle Auto Save. Currently : {(context.IsAutoSaveOn ? "ON" : "OFF")}.");
//            //MenuUI.MenuOption("S", "Save Game", "Save Progress.");
//            //MenuUI.MenuOption("Q", "Quit", "Quit The Game.");
//            //MenuUI.MenuOption("W", "Wipe", "Wipe Your Entire Data!");

//            //Console.Write("\nSelection » ");
//            //string? input = Console.ReadLine()?.ToUpper();
//            string input = "";
//            if (input == "A")
//            {
//                if (context.IsAutoSaveOn)
//                {
//                    context.IsAutoSaveOn = false;
//                    //MenuUI.ColoredMsg(ConsoleColor.Red, "[SYSTEM] Auto Save is now OFF");
//                }
//                else
//                {
//                    context.IsAutoSaveOn = true;
//                    //MenuUI.ColoredMsg(ConsoleColor.Green, "[SYSTEM] Auto Save is now ON");
//                }
//                //Thread.Sleep(1000);
//                return GameState.MainMenu;
//            }

//            if (input == "S")
//            {
//                _saveService.SaveGame(context);
//                return GameState.MainMenu;
//            }

//            return input switch
//            {
//                "0" => GameState.DetailedStats,
//                "1" => GameState.Inventory,
//                "2" => GameState.Blacksmith,
//                "3" => GameState.Training,
//                "4" => GameState.Adventure,
//                "5" => GameState.Dungeon,
//                "6" => GameState.Map,
//                "7" => GameState.Quests,
//                "Q" => GameState.Exit,
//                "W" => GameState.Wipe,
//                _ => GameState.MainMenu
//            };

//        }
//    }
//}
using UnityEngine;

namespace TextBasedRPG.States
{
    public class MainMenuState : IMenuState
    {
        private readonly ISaveService _saveService;

        public MainMenuState(ISaveService saveService)
        {
            _saveService = saveService;
        }

        public GameState Update(GameContext context)
        {
            // Unity'de her karede Save almak performansı düşürür. 
            // Bunu sadece belirli aralıklarla veya butonla yapmalıyız.
            // if (context.IsAutoSaveOn) _saveService.SaveGame(context);

            if (context.Player == null)
            {
                return GameState.HeroSelection;
            }

            // Unity'de burası bir döngü değil, durum kontrolüdür.
            // Şimdilik MainMenu'de kalmaya devam et diyoruz.
            return GameState.MainMenu;
        }

        // Butonlardan çağrılacak yardımcı metotlar ekleyebiliriz
        public void ToggleAutoSave(GameContext context)
        {
            context.IsAutoSaveOn = !context.IsAutoSaveOn;
            Debug.Log($"Auto Save: {context.IsAutoSaveOn}");
        }
    }
} 