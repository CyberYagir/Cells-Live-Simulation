[System.Serializable]
public class CellData {
    public Rotation rotation;
    public byte[] thoughts;
    public byte currentThought = 0;
    public CellKind kind;
    public float energy;
    public string lastAction;
}
