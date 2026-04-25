using TextBasedRPG.Core.Heroes;
using TextBasedRPG.Core.Locations;
using TextBasedRPG.Managers;
using UnityEngine;

public class TravelManager : MonoBehaviour
{
    #region Variable
    [SerializeField] GameObject mapPrefab;
    [SerializeField] GameObject content;
    [SerializeField] GameObject slidePanel;
    Hero p;
    #endregion

    private void OnEnable()
    {
        p = GameManager.Instance.Context.Player;

        if (mapPrefab == null)
        {
            Debug.LogError("Map prefab is not assigned in the inspector.");
            return;
        }

        GenerateCards();
        AdjustAnimPanel();
    }
    public void GenerateCards()
    {
        ClearItems();

        int activeLocIndex = int.Parse(p.ActiveLocation.Substring(1));
        for (int i = 1; i <= p.UnlockedUntill; i++)
        {

            GameObject generatedCard = Instantiate(mapPrefab, content.transform);
            TravelCard travelCard = generatedCard.GetComponent<TravelCard>();
            Location loc = LocationManager.GetLocationByID("L" + i.ToString("D3"));
            
            generatedCard.transform.localScale = Vector3.zero;
            LeanTween.scale(generatedCard, Vector3.one, 0.4f).setDelay(i * 0.1f).setEaseInOutCubic();
            
            if (i == activeLocIndex)
            {
                travelCard.SetUp(loc, UIManager.Instance.IconDB.locationIcon);
                travelCard.button.interactable = false;

            }
            else
            {
                travelCard.SetUp(loc, UIManager.Instance.IconDB.travelIcon);
            }

        }


        int nextLockedIndex = p.UnlockedUntill + 1 ?? 0;

        if (nextLockedIndex <= LocationManager.locations.Count)
        {
            GameObject lockedCardObj = Instantiate(mapPrefab, content.transform);
            TravelCard lockedCard = lockedCardObj.GetComponent<TravelCard>();

            lockedCardObj.transform.localScale = Vector3.zero;
            LeanTween.scale(lockedCardObj, Vector3.one, 0.4f).setDelay((p.UnlockedUntill + 1 ?? 0) * 0.1f).setEaseInOutCubic();
            
            string lockedID = "L" + nextLockedIndex.ToString("D3");
            Location lockedLoc = LocationManager.GetLocationByID(lockedID);

            lockedCard.SetUp(lockedLoc, UIManager.Instance.IconDB.lockedIcon);

            lockedCard.button.interactable = false;
        }

    }
    void AdjustAnimPanel()
    {
        slidePanel.GetComponent<CanvasGroup>().alpha = 0;
    }
    public void Transition()
    {
        CanvasGroup cg = slidePanel.GetComponent<CanvasGroup>();

        float time = 0.5f;

        LeanTween.cancel(slidePanel);
        LeanTween.value(slidePanel, cg.alpha, 1f, time)
            .setEaseInOutCubic()
            .setOnUpdate((float val) =>
            {
                cg.alpha = val;
            }).setOnComplete(() =>
            {
                GenerateCards();
                LeanTween.value(slidePanel, cg.alpha, 0f, time)
                    .setEaseInOutCubic()
                    .setOnUpdate((float val) =>
                    {
                        cg.alpha = val;
                    });
            });
    }
    void ClearItems()
    {
        foreach (Transform child in content.transform) Destroy(child.gameObject);
    }

    
}
