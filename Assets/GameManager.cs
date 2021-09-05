using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class World {
    public float sunEnergy;
    public int maxEnergy;
    public float actionEnergy;
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int fieldSize;
    Cell[,] cells;
    [SerializeField] GameObject cellPrefab;
    List<Cell> cellsPool = new List<Cell>();

    public List<Cell> activeCells = new List<Cell>();
    public World world;
    [HideInInspector]
    public int sun, meat, combined, ticks, generation;

    private void Awake()
    {

        instance = this;
        PoolInit();
        Init();
        StartCoroutine(Ticks());
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StopAllCoroutines();
        }
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
            activeCells.Add(cell);
            return cell;
        }
        return null;
    }

    public void BackToPool(Cell cell)
    {
        cell.UnInit();
        cellsPool.Add(cell);
    }

    public void Init()
    {
        cells = new Cell[fieldSize, fieldSize];
    }
    public void Spawn()
    {
        for (int i = 0; i < 10; i++)
        {
            var pos = new Vector2Int(Random.Range(0, fieldSize), Random.Range(0, fieldSize));
            if (Get(pos) == null)
            {
                Set(pos, GetFromPool());
                Get(pos).WorldInit(pos);
            }
        }
        generation++;
    }
    public Cell Get(Vector2Int pos)
    {
        return cells[pos.x, pos.y];
    }
    public Cell Set(Vector2Int pos, CellCore cell, bool death = false)
    {
        if (death)
        {
            BackToPool(cells[pos.x, pos.y]);
        }
        cells[pos.x, pos.y] = cell as Cell;
        return cells[pos.x, pos.y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(Vector3.zero, new Vector3(fieldSize, 0));
        Gizmos.DrawLine(Vector3.zero, new Vector3(0, fieldSize));
        Gizmos.DrawLine(new Vector3(fieldSize, 0), new Vector3(fieldSize, fieldSize));
        Gizmos.DrawLine(new Vector3(0, fieldSize), new Vector3(fieldSize, fieldSize));
    }

    IEnumerator Ticks()
    {
        while (true)
        {
            if (activeCells.Count == 0) Spawn();
            yield return new WaitForSeconds(1f / 20f);
            sun = 0; meat = 0; combined = 0;
            for (int i = 0; i < activeCells.Count; i++)
            {
                if (activeCells[i] != null)
                {
                    switch (activeCells[i].kind)
                    {
                        case CellKind.Sun:
                            sun++;
                            break;
                        case CellKind.Meat:
                            meat++;
                            break;
                        case CellKind.Combined:
                            combined++;
                            break;
                    }
                    activeCells[i].UpdateCell();
                    if (i % 1000 == 0)
                    {
                        yield return null;
                    }
                }
            }
            UIManager.instance.UpdateUI();
            ticks++;
            yield return null;
        }
    }
}
