using UnityEngine;

[System.Serializable]
public class CellData {
    public Rotation rotation;
    public byte[] thoughts;
    public byte currentThought = 0;
    public CellKind kind;
    public float hp, maxHp;
    public float energy, maxEnergy, moveEnergy;
    public Color genColor, typeColor;
    public bool isDead;
}
