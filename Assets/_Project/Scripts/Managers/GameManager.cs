using Assets._Project.Scripts.UI.Cards;
using System.Collections.Generic;
using System.Linq;
using TextBasedRPG.Core.Entities;
using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Core.Items;
using TextBasedRPG.Core.Locations;
using TextBasedRPG.Core.Shops;
using TextBasedRPG.Managers.DataManagement;
using TextBasedRPG.States;
using UnityEngine;
using Item = TextBasedRPG.Core.Items.Item;
using Material = TextBasedRPG.Core.Items.Material;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private GameState _currentState = GameState.HeroSelection; // Initial Menu
    private Dictionary<GameState, IMenuState> _states; // Game state
    public GameContext Context;
    public ISaveService SaveService;

    void Awake()
    {
        // Singleton
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        SaveService = new DataManager();
        Context = SaveService.LoadGame();

        InitializeStates();
    }
    private void Start()
    {
        if (Context.Player != null)
        {
            _currentState = GameState.MainMenu;
            UIManager.Instance.rightSection.GetComponent<RightSectionManager>().UpdateRightSection(Context);
        }
        else
        {
            _currentState = GameState.HeroSelection;
            UIManager.Instance.rightSection.gameObject.SetActive(false);
            UIManager.Instance.leftSection.gameObject.SetActive(false);
            UIManager.Instance.GenerateClassSelection();
        }
        ChangeState(_currentState);
    }

    private void InitializeStates()
    {
        _states = new Dictionary<GameState, IMenuState> {
            { GameState.HeroSelection, new HeroSelectionState(SaveService) },
            { GameState.MainMenu, new MainMenuState(SaveService) },
            { GameState.DetailedStats, new DetailedStatsState() },
            { GameState.Inventory, new InventoryState() },
            { GameState.Blacksmith, new BlacksmithState() },
            { GameState.Training, new TrainingState() },
            { GameState.Adventure, new AdventureState() },
            { GameState.Map, new MapState() },
            { GameState.Quests, new QuestState() },
            { GameState.Wipe, new WipeState() },
            { GameState.Dungeon, new DungeonState() },
        };
    }

    public void ChangeState(GameState newState)
    {
        if (_states.ContainsKey(newState))
        {
            _currentState = newState;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.SwitchPanel(_currentState);
            }
        }
    }
}
[System.Serializable]
public class GameContext
{
    public Hero Player { get; set; }
    public bool IsAutoSaveOn { get; set; }
    public List<Entity> Entities { get; set; }
    public List<Location> Locations { get; set; }
    public List<Weapon> Weapons { get; set; }
    public List<Armor> Armors { get; set; }
    public List<Material> Materials { get; set; }
    public List<Consumable> Consumables { get; set; }
    public List<Shop> Shops { get; set; }
    public Dictionary<string, string> ClassWeaponCheck { get; private set; } = new();
    public Dictionary<string, Item> MasterItemBook { get; private set; } = new();
    public void InitializeMasterBook()
    {
        var all = Weapons!.Cast<Item>()
            .Concat(Armors!)
            .Concat(Materials!)
            .Concat(Consumables!);

        MasterItemBook = all.ToDictionary(item => item.ID, item => item);
    }
    public void InitializeClassWeaponCheck()
    {
        // Gotta make this dynamic later
        ClassWeaponCheck = new Dictionary<string, string>
            {
                { "Warrior", "Sword" },
                { "Archer", "Bow" },
                { "Mage", "Staff" }
            };
    }
}