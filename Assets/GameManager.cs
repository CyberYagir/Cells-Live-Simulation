using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class World {
    public float sunEnergy;
    public float rotateEnergy;
    public float moveEnergy;
    public int maxEnergy;
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] int fieldSize;
    Cell[,] cells;
    [SerializeField] GameObject cellPrefab;
    List<Cell> cellsPool = new List<Cell>();
    public World world;

    private void Awake()
    {
        instance = this;
        PoolInit();
        Init();

        StartCoroutine(Ticks());
    }

    public void PoolInit()
    {
        for (int i = 0; i < fieldSize * fieldSize; i++)
        {
            var inst = Instantiate(cellPrefab, transform);
            Cell cell = new Cell(inst.transform);
            cellsPool.Add(cell);
        }
    }
    public Cell GetFromPool()
    {
        if (cellsPool.Count > 0)
        {
            var cell = cellsPool[0];
            cellsPool.RemoveAt(0);
            return cell;
        }
        return null;
    }
    public void Init()
    {
        cells = new Cell[fieldSize, fieldSize];
        var pos = new Vector2Int(Random.Range(0, fieldSize), Random.Range(0, fieldSize));
        Set(pos, GetFromPool());
        Get(pos).WorldInit(pos);
    }
    public Cell Get(Vector2Int pos)
    {
        return cells[pos.x, pos.y];
    }
    public Cell Set(Vector2Int pos, Cell cell)
    {
        cells[pos.x, pos.y] = cell;
        return cells[pos.x, pos.y];
    }

    IEnumerator Ticks()
    {
        while (true)
        {
            for (int i = 0; i < 20; i++)
            {
                for (int x = 0; x < cells.GetLength(0); x++)
                {
                    for (int y = 0; y < cells.GetLength(1); y++)
                    {
                        if (cells[x, y] != null)
                            cells[x, y].UpdateCell();
                    }
                }
                yield return new WaitForSeconds(1f / 20f);
            }
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                for (int y = 0; y < cells.GetLength(1); y++)
                {
                    if (cells[x, y] != null)
                        cells[x, y].Brain();
                }
            }
        }
    }

}
