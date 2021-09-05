using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GenTypes {HP, DUB, ENRGB, SUNBONUS, MAXENERGY}
[System.Serializable]
public class GenIs
{
    public GenTypes genTypes;
    public int genID;
    public AnimationCurve value;
    public float bonus;
}
public class CellBonuses : MonoBehaviour
{
    public List<GenIs> gens = new List<GenIs>();

    public float GetValue(CellData cell, GenTypes type)
    {
        var n = gens.Find(x => x.genTypes == type);
        return n.value.Evaluate(cell.thoughts[n.genID]) * n.bonus;
    }
}
