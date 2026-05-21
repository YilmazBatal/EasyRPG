using Assets._Project.Scripts.UI;
using Assets._Project.Scripts.UI.Cards;
using System.Collections;
using System.Collections.Generic;
using TextBasedRPG.Events;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Essential Objects")]
    [SerializeField] private Transform canvas;
    [SerializeField] private Transform middleSection;
    [SerializeField] public Transform rightSection;
    [SerializeField] public Transform leftSection;
    [SerializeField] public Transform popupHolder;

    [SerializeField] public Image raycastBlocker1;
    [SerializeField] public Image raycastBlocker2;

    [Header("Panels Mapping")]
    [SerializeField] private List<PanelMapping> panels;
    private Dictionary<GameState, GameObject> _panelDictionary;
    private GameObject _activePanel;

    [SerializeField] private GameObject _popUp;
    [SerializeField] private GameObject _heroSelectionContent;
    [SerializeField] private GameObject _classSelection;
    [HideInInspector] public GameObject _activePopUp;

    public Dictionary<string, Color> rarityColors;

    [Header("Configuration")]
    [SerializeField] [Range(0.01f,0.5f)] static float typeWriterSpeed = 0.015f;

    [Header("Icon Database")]
    [SerializeField] private IconDatabase iconDB;

    [Header("Region Panels")]
    [SerializeField] private LocationSO[] locations;

    [SerializeField] private Image _town, _adventure, _inventory, _training, _blacksmith, _tradeCenter, _dungeon, _map, _quest, _insights, _settings;

    public IconDatabase IconDB => iconDB;

    [System.Serializable]
    public struct PanelMapping
    {
        public GameState state;
        public GameObject panelObject;
    }

    private void OnEnable()
    {
        if (GameManager.Instance?.Context?.Player?.ActiveLocation != null)
            BackgroundConfiguration(GameManager.Instance.Context);
        EventManager.HeroEvents.OnLocationChanged += BackgroundConfiguration;
    }
    private void OnDisable()
    {
        EventManager.HeroEvents.OnLocationChanged -= BackgroundConfiguration;
    }

    void Awake()
    {
        Instance = this;
        _panelDictionary = new Dictionary<GameState, GameObject>();
        foreach (var p in panels) _panelDictionary.Add(p.state, p.panelObject);
        foreach (var p in panels)
        {
            p.panelObject.SetActive(false);
        }
            InitializeRarityColors();
    }

    public void SwitchPanel(GameState newState)
    {
        if (!_panelDictionary.ContainsKey(newState)) return;

        var oldState = _activePanel;

        if (_activePanel != null) _activePanel.SetActive(false);

        _activePanel = _panelDictionary[newState];
        _activePanel.SetActive(true);

        //LeanTween.cancel(oldState);
        //LeanTween.cancel(_activePanel);

        if (oldState == null) return;
        else
        {
            LeanTween.value(oldState.gameObject, 1f, 0f, 0.2f).setEaseInOutCubic().setOnUpdate((float val) =>
            {
                oldState.GetComponent<CanvasGroup>().alpha = val;
            });
            LeanTween.value(_activePanel.gameObject, 0f, 1f, 0.2f).setEaseInOutCubic().setOnUpdate((float val) =>
            {
                _activePanel.GetComponent<CanvasGroup>().alpha = val;
            });
        }
    }
    void BackgroundConfiguration(GameContext context, bool setDelay = false)
    {
        StartCoroutine(BgConfigCoroutine(context, setDelay));
    }
    IEnumerator BgConfigCoroutine(GameContext context, bool setDelay = false)
    {
        if (context?.Player?.ActiveLocation == null)
            yield break;

        string rawID = GameManager.Instance.Context.Player.ActiveLocation; // "L003"
        int idNumber = int.Parse(rawID.Substring(1)); // Result : 3

        LocationSO activeBackground = locations[idNumber - 1];

        if (setDelay)
            yield return new WaitForSeconds(0.5f);

        ApplyBG(activeBackground);

        yield return null;


    }
    private void ApplyBG(LocationSO data)
    {
        _town.sprite = data.townBG;
        _adventure.sprite = data.adventureBG;
        _inventory.sprite = data.inventoryBG;
        _training.sprite = data.trainingBG;
        _tradeCenter.sprite = data.tradecenterBG;
        _blacksmith.sprite = data.blacksmithBG;
        _quest.sprite = data.townBG;
        _map.sprite = data.adventureBG;
        _dungeon.sprite = data.adventureBG;
        _insights.sprite = data.townBG;
    }
    public void GeneratePopUp(string title, string description, UnityAction onConfirm)
    {
        GameObject PopUp = Instantiate(_popUp);
        PopUpManager Config = PopUp.GetComponent<PopUpManager>();
        PopUp.transform.SetParent(popupHolder, false);

        PopUp.SetActive(true);

        Config.title.text = title;
        Config.description.text = description;

        Config.confirm.onClick.RemoveAllListeners();
        Config.confirm.onClick.AddListener(onConfirm);

        _activePopUp = PopUp;

        raycastBlocker1.gameObject.SetActive(true);
        LeanTween.value(raycastBlocker1.gameObject, 0f, 0.5f, 0.2f).setEaseInOutCubic().setOnUpdate((float val) =>
        {
            raycastBlocker1.color = new Color(0f, 0f, 0f, val);
        });
        PopUp.transform.localScale = Vector3.zero;
        LeanTween.value(PopUp, 0f, 1f, 0.3f).setEaseInOutCubic().setOnUpdate((float val) =>
        {
            PopUp.transform.localScale = new Vector3(val, val, val);
        });
    }
    public void GenerateClassSelection()
    {
        var heroDb = Resources.Load<HeroDatabase>("HeroDatabase");

        for (int i = 0; i < heroDb.allHeroes.Count; i++)
        {
            GameObject ClassSelection = Instantiate(_classSelection);
            ClassSelection.transform.SetParent(_heroSelectionContent.transform, false);

            HeroCardManager Config = ClassSelection.GetComponent<HeroCardManager>();

            Config.title.text = heroDb.allHeroes[i].className;
            Config.description.text = heroDb.allHeroes[i].description;
            Config.icon.sprite = heroDb.allHeroes[i].classIcon;
            Config.atk.text = heroDb.allHeroes[i].atk.ToString();
            Config.def.text = heroDb.allHeroes[i].def.ToString();
            Config.hp.text = heroDb.allHeroes[i].hp.ToString();
            Config.spd.text = heroDb.allHeroes[i].spd.ToString();
            Config.btn.GetComponent<HeroSelectionUI>().heroData = heroDb.allHeroes[i];
        }
    }
    private void InitializeRarityColors()
    {
        rarityColors = new Dictionary<string, Color>
        {
            { "Common", new Color(100f/255f, 100f/255f, 100f/255f) }, 
            { "Uncommon", new Color(50f/255f, 205f/255f, 50f/255f) }, 
            { "Rare", new Color(0f/255f, 160f/255f, 255f/255f) }, 
            { "Epic", new Color(160f/255f, 32f/255f, 240f/255f) }, 
            { "Legendary", new Color(255f/255f, 140f/255f, 0f/255f) }
        };
    }
    
    public static IEnumerator BruteForceTypeWriterRoutine(TMP_Text textComponent, string fullText)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*";
        textComponent.text = "";
        string currentDisplayedText = "";

        for (int i = 0; i < fullText.Length; i++)
        {
            if (fullText[i] == '<')
            {
                int tagCloseIndex = fullText.IndexOf('>', i);
                if (tagCloseIndex != -1)
                {
                    string tag = fullText.Substring(i, tagCloseIndex - i + 1);
                    currentDisplayedText += tag;
                    textComponent.text = currentDisplayedText;
                    i = tagCloseIndex; 
                    continue;
                }
            }

            char targetChar = fullText[i];

            if (targetChar == ' ')
            {
                currentDisplayedText += " ";
                textComponent.text = currentDisplayedText;
                continue;
            }

            for (int j = 0; j < 3; j++)
            {
                textComponent.text = currentDisplayedText + chars[Random.Range(0, chars.Length)];
                yield return new WaitForSeconds(typeWriterSpeed);
            }

            float pitch = Random.Range(0.75f, 0.9f);
            AudioManager.Instance.PlayUI(AudioManager.Instance.audioDB.typewriter, pitch);
            currentDisplayedText += targetChar;
            textComponent.text = currentDisplayedText;
        }
    } 
}