using UnityEngine;

[CreateAssetMenu(fileName = "HeroData", menuName = "RPG/HeroData")]
public class HeroData : ScriptableObject
{
    public string className;
    [TextArea] public string description;
    public Sprite classIcon; // UI'da göstermek için

    [Header("Base Stats")]
    public int hp;
    public int atk;
    public int def;
    public int spd;
}