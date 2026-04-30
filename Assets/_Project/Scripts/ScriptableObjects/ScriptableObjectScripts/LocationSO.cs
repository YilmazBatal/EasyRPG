using UnityEngine;

[CreateAssetMenu(fileName = "LocationSO", menuName = "UI/Location")]
public class LocationSO : ScriptableObject
{
    public string locationID;
    public string locationName;

    [Header("Visuals")]
    public Sprite townBG;
    public Sprite adventureBG;
    public Sprite inventoryBG;
    public Sprite trainingBG;
    public Sprite blacksmithBG;
    public Sprite tradecenterBG;
}
