using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroDatabase", menuName = "RPG/Hero Database")]
public class HeroDatabase : ScriptableObject
{
    public List<HeroData> allHeroes;

    public HeroData GetHeroByName(string className)
    {
        return allHeroes.Find(x => x.className == className);
    }
}