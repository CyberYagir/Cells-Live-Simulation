using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Range
{
    public float min, max;

    public float GetRandom()
    {
        return Random.Range(min, max);
    }
}

[System.Serializable]
public class TypeDeviders
{
    public float sun = 1, meat = 3, combined = 2;
}

[CreateAssetMenu(fileName = "World", menuName = "Game/World Object", order = 1)]
public class WorldObject : ScriptableObject
{
    public int fieldSize = 200;
    public float sunEnergy = 1.5f;
    public int maxEnergy = 30;
    public float actionEnergy = 10;
    public float deathSpeed = 0.1f;
    public int heathMultiply = 10;
    [Range(0, 100)] public int duplicateRarity = 50;
    public Range healthRandom = new Range() { min = 1, max = 1.5f};
    public float sunGenDevider = 10;
    public float predatorTimeBoost = 0.2f;
    public TypeDeviders kindsEnergyDeviders = new TypeDeviders() { sun = 1, combined = 2, meat = 3};
    public Color deathColor = new Color(0.2f, 0.2f, 0.2f, 1);
    public int isBrotherDifference = 25;
    public int startCount = 10;
    public bool isEnableTransfer = false;

}
