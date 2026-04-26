using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Core.Locations;
using TextBasedRPG.Events;
using TextBasedRPG.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TravelCard : MonoBehaviour
{
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text desc;
    [SerializeField] TMP_Text tagTitle;
    [SerializeField] Image tagColor;
    [SerializeField] Image buttonIcon;
    [SerializeField] public Button button;
    private TravelManager travelManager;
    Hero p => GameManager.Instance.Context.Player;

    private void OnEnable()
    {
        travelManager = FindFirstObjectByType<TravelManager>();
    }

    public void SetUp(Location location, Sprite icon)
    {
        title.text = location.Name;
        desc.text = location.Description;
        buttonIcon.sprite = icon;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => Travel(location.ID));
    }
    void Travel(string locationID)
    {
        p.ActiveLocation = locationID;
        Toaster.Instance.ShowToast($"Traveled to {LocationManager.GetLocationName(locationID)}", UIManager.Instance.IconDB.confirmIcon);
        travelManager.Transition();
        EventManager.HeroEvents.TriggerLocationChanged(GameManager.Instance.Context, true);
    }
}
