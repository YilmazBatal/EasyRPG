public class AdventureState : IMenuState
{
    const int adventureTextChance = 20;         // 20%
    const int monsterEncounterChance = 80;      // 60%
    const int itemFindingChance = 85;           // 5%
    const int miniEventChance = 95;             // 10%
    const int chestChance = 100;                 // 5%
    //const int merchantEncounterChance = 0; // in the future
    public bool isAdventuring = true;
    public GameState Update(GameContext context)
    {
        //Console.Clear();
        //isAdventuring = true;
        //while (isAdventuring)
        //{
        //    Traveling();
        //    switch (AdventureGenerator())
        //    {
        //        case <= adventureTextChance:
        //            var currentLocationasObject = context.Locations?.FirstOrDefault(r => r.ID == context.Player?.ActiveLocation);
        //            string randomText = currentLocationasObject!.AdventureTexts![Random.Shared.Next(0, currentLocationasObject.AdventureTexts.Count + 1)];
        //            BruteForceTypeWriter(randomText);
        //            break;
        //        case <= monsterEncounterChance:
        //            CombatManager.StartCombat(context);
        //            break;
        //        case <= itemFindingChance:
        //            GenerateItem();
        //            break;
        //        case <= miniEventChance:
        //            GenerateEvent();
        //            break;
        //        case <= chestChance:
        //            GenerateEvent();
        //            break;
        //        default:
        //            Console.WriteLine("There was a problem.");
        //            break;
        //    }
        //    Console.WriteLine("\n[1] Keep Adventuring | [Any Other Key] Go Back to Town");
        //    string? input = Console.ReadLine();
        //    if (input != "1")
        //        isAdventuring = false;
        //}
        return GameState.MainMenu;
    }

    //private static void Traveling()
    //{
    //    string travelText = "Traveling";
    //    for (int i = 0; i < Random.Shared.Next(2, 5); i++)
    //    {
    //        Console.Clear();
    //        travelText += ".";
    //        Console.WriteLine(travelText);
    //        Thread.Sleep(400);
    //    }
    //    travelText = "Traveling";
    //    Console.Clear();
    //}

    //public void GenerateItem()
    //{
    //    // add items to location json and when the player finds an item get the item from the location json and add it to the player inventory
    //    Console.WriteLine("You have found an item");
    //    Console.WriteLine("Item given to the player");
    //}
    //public void GenerateEvent()
    //{
    //    Console.WriteLine("You have encountered a little girl");
    //    Console.WriteLine("wanna help her? y/n");
    //}
    //public void GenerateChest()
    //{
    //    Console.WriteLine("You have encountered a chest");
    //    Console.WriteLine("player got this gold and this exp ");
    //}

    //public int AdventureGenerator()
    //{
    //    return Random.Shared.Next(0, 101);
    //}

    //public static void TypeWriter(string text, int speed = 50)
    //{
    //    foreach (char c in text)
    //    {
    //        Console.Write(c);
    //        Thread.Sleep(speed);
    //    }
    //    Console.WriteLine();
    //}
    //public static void BruteForceTypeWriter(string text, int delay = 10)
    //{
    //    string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=";
    //    string currentDisplay = "";
    //    foreach (char c in text)
    //    {
    //        if (c == ' ')
    //        {
    //            currentDisplay += ' ';
    //            Console.Write(" ");
    //        }
    //        else
    //        {
    //            for (int i = 0; i < 3; i++) 
    //            {
    //                char randomChar = chars[Random.Shared.Next(chars.Length)];
    //                Console.Write(randomChar);
    //                Thread.Sleep(delay);
    //                Console.Write("\b");
    //            }

    //            currentDisplay += c;
    //            Console.Write(c);
    //        }
    //    }
    //    Console.WriteLine();
    //}
}