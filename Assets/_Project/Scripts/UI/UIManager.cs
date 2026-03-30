using Assets._Project.Scripts.UI;
using Assets._Project.Scripts.UI.Cards;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] [InspectorRange(0.01f,0.03f)] float typeWriterSpeed = 0.015f;

    [System.Serializable]
    public struct PanelMapping
    {
        public GameState state;
        public GameObject panelObject;
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
    private void Start()
    {
        //foreach (var p in panels) p.panelObject.SetActive(false);
    }

    public void SwitchPanel(GameState newState)
    {
        if (!_panelDictionary.ContainsKey(newState)) return;

        if (_activePanel != null) _activePanel.SetActive(false);

        _activePanel = _panelDictionary[newState];
        _activePanel.SetActive(true);
    }

    public void GeneratePopUp(string title, string description, UnityAction onConfirm)
    {
        GameObject PopUp = Instantiate(_popUp);
        PopUp.transform.SetParent(middleSection.transform, false);
        PopUpManager Config = PopUp.GetComponent<PopUpManager>();

        Config.title.text = title;
        Config.description.text = description;

        Config.confirm.onClick.RemoveAllListeners();
        Config.confirm.onClick.AddListener(onConfirm);

        _activePopUp = PopUp;

        raycastBlocker1.gameObject.SetActive(true);
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
            { "Common", new Color(120f/255f, 122f/255f, 123f/255f) },
            { "Uncommon", new Color(89f/255f, 197f/255f, 102f/255f) },
            { "Rare", new Color(64f/255f, 201f/255f, 255/255f) },
            { "Epic", new Color(187f/255f, 85f/255f, 255/255f) },
            { "Legendary", new Color(1f, 0.75f, 0f) }
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
                yield return new WaitForSeconds(0.01f);
            }

            currentDisplayedText += targetChar;
            textComponent.text = currentDisplayedText;
        }
    }
}