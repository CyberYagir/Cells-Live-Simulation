using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int fieldSize { get; private set; }
    [SerializeField] private GameObject cellPrefab;
    private List<Cell> cellsPool = new List<Cell>();
    private Cell[,] cells;
    
    public WorldObject world;
    
    
     public List<Cell> activeCells = new List<Cell>();
    [HideInInspector] public int sun, meat, combined, ticks, generation;



    private void InitCamera()
    {
        var cam = FindObjectOfType<RenderTextureCreator>().GetComponent<Camera>();
        cam.transform.position = new Vector3(fieldSize / 2f, fieldSize / 2f, -10f) - (Vector3.one / 2f);
        cam.orthographicSize = fieldSize / 2f;
    }
    
    private void Awake()
    {
        Instance = this;

        world = SaveManager.LoadWorld();

        fieldSize = world.fieldSize;
        
        InitCamera();
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

    private void PoolInit()
    {
        cellsPool = new List<Cell>(fieldSize * fieldSize);
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
            var cell = cellsPool[cellsPool.Count-1];
            cellsPool.RemoveAt(cellsPool.Count-1);
            activeCells.Add(cell);
            return cell;
        }

        return null;
    }

    private void BackToPool(Cell cell)
    {
        cell.UnInit();
        cellsPool.Add(cell);
        cells[cell.posInArray.x, cell.posInArray.y] = null;
    }

    private void Init()
    {
        cells = new Cell[fieldSize, fieldSize];
        Spawn();
    }
    private void Spawn()
    {
        for (int i = 0; i < world.startCount; i++)
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
    private void BackDeadCells()
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
    public Cell Set(Vector2Int pos, CellCore cell, bool death = false, bool isEating = false)
    {
        if (death)
        {
            if (cells[pos.x, pos.y] != null)
            {
                BackToPool(cells[pos.x, pos.y]);
                return null;
            }
        }else
        if (isEating && cell == null && cells[pos.x, pos.y] != null)
        {
            cells[pos.x, pos.y].isDead = true;
            BackToPool(cells[pos.x, pos.y]);
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
