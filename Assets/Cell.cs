using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rotation { Left, Up, Right, Down};
public enum CellKind { Sun, Meat, Combined};


[System.Serializable]
public class CellData {
    public Rotation rotation;
    public byte[] thoughts;
    public byte currentThought = 0;
    public CellKind kind;
    public float energy;
    public string lastAction;
}

public abstract class CellCore{

    protected Rotation rotation;
    protected byte[] thoughts;
    protected byte currentThought = 0;
    protected byte thoughtsLength = 16;
    protected byte thoughtsMax = 10;
    protected Transform cellInWorld;
    protected CellKind kind;
    protected float energy;
    protected CellBonuses cellBonuses;
    protected float hp = 100;


    protected World world;
    public Vector2Int posInArray;
    protected bool isDead;

    protected string lastAction = "None";
    public void Death()
    {
        isDead = true;
        GameManager.instance.activeCells.RemoveAt(GameManager.instance.activeCells.FindIndex(x => x == this));
        cellInWorld.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
    }
    public void Move(Vector3 dir)
    {
        bool can = true;

        var nextPos = CalcPos(posInArray + Vector2Int.RoundToInt(dir));
        if (!CheckIsCellImpty(dir))
        {
            if (GameManager.instance.Get(nextPos).isDead == false)
            {
                can = false;
            }
        }

        if (can)
        {
            bool isEated = EatDead(nextPos);

            GameManager.instance.Set(CalcPos(posInArray), null);
            GameManager.instance.Set(nextPos, this);
            posInArray = CalcPos(posInArray + Vector2Int.RoundToInt(dir));
            energy -= world.actionEnergy;
            energy += isEated ? world.maxEnergy / 2f : 0;
        }
    }
    public void Dublicate(Vector3 dir)
    {
        bool can = true;
        var nextPos = CalcPos(posInArray + Vector2Int.RoundToInt(dir));
        if (!CheckIsCellImpty(dir))
        {
            if (GameManager.instance.Get(nextPos).isDead == false)
            {
                can = false;
            }
        }
        if (can)
        {
            var isEated = EatDead(nextPos);
            var newPos = nextPos;
            var newCell = GameManager.instance.Set(newPos, GameManager.instance.GetFromPool());
            newCell.WorldInit(newPos, this);
            newCell.posInArray = nextPos;
            newCell.energy += isEated ? world.maxEnergy / 2f : 0;

            energy -= world.actionEnergy;
        }
    }
    public bool EatDead(Vector2Int nextPos)
    {
        if (GameManager.instance.Get(nextPos) != null)
        {
            if (GameManager.instance.Get(nextPos).isDead)
            {
                hp += 50;
                energy += world.maxEnergy;
                GameManager.instance.Set(nextPos, null);
                return true;
            }
        }
        return false;
    }




    public bool CheckIsCellImpty(Vector3 direction)
    {
        return GameManager.instance.Get(CalcPos(posInArray + Vector2Int.RoundToInt(direction))) == null;
    }
    public Vector2Int CalcPos(Vector2Int pos)
    {
        var newPos = pos;
        if (pos.y >= GameManager.instance.fieldSize)
        {
            newPos = new Vector2Int(newPos.x, 0);
        }
        if (pos.x >= GameManager.instance.fieldSize)
        {
            newPos = new Vector2Int(0, newPos.y);
        }

        if (pos.y < 0)
        {
            newPos = new Vector2Int(newPos.x, GameManager.instance.fieldSize - 1);
        }
        if (pos.x < 0)
        {
            newPos = new Vector2Int(GameManager.instance.fieldSize - 1, newPos.y);
        }
        return newPos;
    }
    public CellData GetCellData()
    {
        return new CellData() { currentThought = currentThought, rotation = rotation, energy = energy, kind = kind, thoughts = thoughts, lastAction = lastAction};
    }


    public void print(object obj)
    {
        Debug.Log(obj.ToString());
    }
}

[System.Serializable]
public class Cell : CellCore
{
    public Cell(Transform cell)
    {
        cellInWorld = cell;
        cell.gameObject.SetActive(false);

        thoughts = new byte[thoughtsLength];
        for (int i = 0; i < thoughts.Length; i++)
        {
            thoughts[i] = (byte)Random.Range(1, thoughtsMax);
            if (thoughts[i] == thoughts.Length)
            {
                thoughts[i]++;
            }
        }
        currentThought = 0;
        rotation = (Rotation)Random.Range(0, 4);

        world = GameManager.instance.world;
    }

    public void WorldInit(Vector2Int pos, CellCore parent = null)
    {
        cellInWorld.gameObject.SetActive(true);
        cellInWorld.position = (Vector2)pos;
        posInArray = pos;
        cellBonuses = cellInWorld.GetComponent<CellBonuses>();
        if (parent != null)
        {
            thoughts = (parent as Cell).thoughts;
            if (Random.Range(0, 10) == 1)
                thoughts[Random.Range(0, thoughtsLength)] = (byte)Random.Range(0, thoughtsMax);
        }
        float r = 0, g = 0, b = 0;
        for (int i = 0; i < thoughtsLength; i++)
        {
            if (i < 3)
            {
                r += thoughts[i];
            }else if (i >= 3 && i <= 7)
            {
                g += thoughts[i];
            }
            else
            {
                b += thoughts[i];
            }
        }
        float max = Mathf.Max(Mathf.Max(r, g), b);
        cellInWorld.GetComponent<SpriteRenderer>().color = new Color(r / max, g / max, b / max);

        hp = cellBonuses.GetValue(GetCellData(), GenTypes.HP);
    }

    public void Brain()
    {
        if (thoughts[currentThought] == 4)
        {
            DublicateCell();
        }
        if (thoughts[currentThought] == 6)
        {
            MoveCell();
        }
        if (thoughts[currentThought] == 9)
        {
            RotateCell();
        }
        GoToNextThought();
    }
    public void GoToNextThought()
    {
        currentThought++;
        if (currentThought >= thoughtsLength)
        {
            currentThought = 0;
        }
    }


    public void RotateCell()
    {
        if (energy > world.actionEnergy)
        {
            energy -= world.actionEnergy;
            int rotate = (int)rotation;
            rotate += Random.Range(-1, 2);
            if (rotate >= 4) rotate = 0;
            if (rotate < 0) rotate = 3;
            rotation = (Rotation)rotate;
            lastAction = "Rotate";
        }
    }
    public void DublicateCell()
    {
        if (energy > world.actionEnergy)
        {
            switch (rotation)
            {
                case Rotation.Left:
                    Dublicate(Vector3.left);
                    break;
                case Rotation.Up:
                    Dublicate(Vector3.up);
                    break;
                case Rotation.Right:
                    Dublicate(Vector3.right);
                    break;
                case Rotation.Down:
                    Dublicate(Vector3.down);
                    break;
            }
            lastAction = "Dublicate";
        }
    }
    public void MoveCell()
    {
        if (energy > world.actionEnergy)
        {
            switch (rotation)
            {
                case Rotation.Left:
                    Move(Vector3.left);
                    break;
                case Rotation.Up:
                    Move(Vector3.up);
                    break;
                case Rotation.Right:
                    Move(Vector3.right);
                    break;
                case Rotation.Down:
                    Move(Vector3.down);
                    break;
            }
            lastAction = "Move";
        }
    }


    public void UpdateCell()
    {
        hp -= 0.1f;
        if (hp <= 0)
        {
            Death();
        }
        if (!isDead)
        {
            switch (kind)
            {
                case CellKind.Sun:
                    SunCellUpdate();
                    break;
                case CellKind.Meat:
                    MeatCellUpdate();
                    break;
                case CellKind.Combined:
                    MeatCellUpdate();
                    break;
                default:
                    break;
            }
            var oldPos = (Vector2)posInArray;
            Brain();
            if (Vector2.Distance(posInArray, oldPos) > 5)
            {
                cellInWorld.position = (Vector2)posInArray;
            }
            cellInWorld.GetComponent<SmoothMover>().newPos = (Vector2)posInArray;
        }
    }
    public void SunCellUpdate()
    {
        energy += world.sunEnergy;
        if ((int)energy > world.maxEnergy)
        {
            energy = world.maxEnergy;
        }
    }
    public void MeatCellUpdate()
    {

    }
    public void CombinedCellUpdate()
    {
        SunCellUpdate();
        MeatCellUpdate();
    }

}
