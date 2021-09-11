using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class World {
    public float sunEnergy;
    public int maxEnergy;
    public float actionEnergy;
    public float deathSpeed;
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int fieldSize;
    Cell[,] cells;
    [SerializeField] GameObject cellPrefab;
    List<Cell> cellsPool = new List<Cell>();
    [HideInInspector]
    public List<Cell> activeCells = new List<Cell>();
    public World world;
    [HideInInspector]
    public int sun, meat, combined, ticks, generation;

    public int cellsPoolDisplay, activeCellsDisplay, deadCellsDisplay;

    private void Awake()
    {
        fieldSize = PlayerPrefs.GetInt("Size", 200);
        var cam = FindObjectOfType<RenderTextureCreator>().GetComponent<Camera>();
        cam.transform.position = new Vector3(fieldSize / 2f, fieldSize / 2f, -10f) - (Vector3.one / 2f);
        cam.orthographicSize = fieldSize / 2;

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
        cellsPoolDisplay = cellsPool.Count;
        activeCellsDisplay = activeCells.Count;
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
        cells[cell.posInArray.x, cell.posInArray.y] = null;
    }

    public void Init()
    {
        cells = new Cell[fieldSize, fieldSize];
        Spawn();
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
    public void BackDeadCells()
    {
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                Set(new Vector2Int(x, y), null, true);
            }
        }
    }

    public Cell Get(Vector2Int pos)
    {
        return cells[pos.x, pos.y];
    }
    public Cell Set(Vector2Int pos, CellCore cell, bool death = false)
    {
        if (death)
        {
            if (cells[pos.x, pos.y] != null)
            {
                BackToPool(cells[pos.x, pos.y]);
                return null;
            }
        }
        cells[pos.x, pos.y] = (cell as Cell);
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
            if (activeCells.Count == 0)
            {
                BackDeadCells();
                Spawn();
            }
            yield return new WaitForSeconds(1f / 20f);
            sun = 0; meat = 0; combined = 0;

            List<int> removes = new List<int>();
            for (int i = 0; i < activeCells.Count; i++)
            {
                if (activeCells[i] != null)
                {
                    activeCells[i].UpdateCell();
                    if (activeCells[i].isDead == false)
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
                    }
                    else
                    {
                        removes.Add(i);
                    }
                    if (i % 1000 == 0)
                    {
                        yield return null;
                    }
                }
                else
                {
                    removes.Add(i);
                }
            }
            
            for (int i = 0; i < removes.Count; i++)
            {
                activeCells.RemoveAt(removes[i]-i);
            }

            UIManager.instance.UpdateUI();
            ticks++;
            yield return null;
        }
    }


}
