using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public abstract class CellCore
{

    protected Rotation rotation;
    protected byte[] thoughts;
    protected byte currentThought = 0;
    protected byte thoughtsLength = 16;
    protected byte thoughtsMax = 10;
    protected Transform cellInWorld;
    public CellKind kind;
    protected float energy;
    protected float hp = 100;
    protected float maxhp = 100;


    protected World world;
    public Vector2Int posInArray;
    public bool isDead;

    protected SmoothMover smoothMover;

    protected string lastAction = "None";

    public float actionEnergy {
        get
        {
            return (world.actionEnergy / (kind == CellKind.Meat ? 3 : 1)) / (kind == CellKind.Combined ? 2 : 1);
        }
    }



    public CellData GetCellData()
    {
        return new CellData() { currentThought = currentThought, rotation = rotation, energy = energy, kind = kind, thoughts = thoughts };
    }

    public void Death()
    {
        isDead = true;
        cellInWorld.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 1);
    }
    public void TransferEnergy(Vector2Int dir)
    {
        var nextPos = CalcPos(posInArray + dir);
        if (!CheckIsCellImpty((Vector2)dir))
        {
            var cell = GameManager.instance.Get(nextPos);
            if (!IsntBrotherCell(cell))
            {
                cell.energy += actionEnergy;
                energy -= actionEnergy;
            }
        }
    }
    public bool Move(Vector3 dir)
    {
        if (energy > actionEnergy)
        {
            var nextPos = CalcPos(posInArray + Vector2Int.RoundToInt(dir));
            if (CheckIsCellImpty(dir) || Eat(dir))
            {
                GameManager.instance.Set(nextPos, this);
                GameManager.instance.Set(CalcPos(posInArray), null);
                posInArray = nextPos;
                energy -= actionEnergy;
                return true;
            }
        }
        return false;
    }

    public bool Eat(Vector3 dir)
    {
        var nextPos = CalcPos(posInArray + Vector2Int.RoundToInt(dir));
        var nearCell = GameManager.instance.Get(nextPos);
        if (nearCell != null)
        {
            if (kind == CellKind.Sun)
            {
                if (nearCell.isDead)
                {
                    GameManager.instance.Set(nextPos, null, true);
                    energy += actionEnergy;
                    return true;
                }
            }
            else
            {
                if (nearCell.isDead)
                {
                    GameManager.instance.Set(nextPos, null, true);
                    energy += actionEnergy;
                    return true;
                }
                else
                {
                    if (!IsntBrotherCell(nearCell))
                    {
                        energy += actionEnergy + nearCell.energy;
                        nearCell.isDead = true;
                        GameManager.instance.Set(nextPos, null, true);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool Dublicate(Vector3 dir)
    {
        var newPos = CalcPos(posInArray + Vector2Int.RoundToInt(dir));
        if (CheckIsCellImpty(dir))
        {
            var newCell = GameManager.instance.Set(newPos, GameManager.instance.GetFromPool());
            newCell.WorldInit(newPos, this);
            newCell.posInArray = newPos;
            newCell.energy = actionEnergy;
            energy -= actionEnergy;
            return true;
        }
        else
        {
            if (GameManager.instance.Get(newPos).isDead)
            {
                Move(dir);
                return false;
            }
        }
        return false;
    }

    public bool IsntBrotherCell(Cell nearCell)
    {
        return (nearCell.CollectGens() - CollectGens() > 50);
    }

    public int CollectGens()
    {
        return thoughts.Sum(x => x);
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
   


    public void print(object obj)
    {
        Debug.Log(obj.ToString());
    }
}
