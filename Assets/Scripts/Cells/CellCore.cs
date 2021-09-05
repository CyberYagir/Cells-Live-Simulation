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
    protected CellBonuses cellBonuses;
    protected float hp = 100;


    protected World world;
    public Vector2Int posInArray;
    public bool isDead;

    protected string lastAction = "None";

    protected float maxEnergy
    {
        get {
            return world.maxEnergy * cellBonuses.GetValue(GetCellData(), GenTypes.MAXENERGY);
        }
    }

    protected float actionEnergyMove
    {
        get
        {
            return world.actionEnergy / (kind == CellKind.Meat ? 5 : 1) / (kind == CellKind.Combined ? 2 : 1);
        }
    }

    public void Death()
    {
        isDead = true;
        GameManager.instance.activeCells.Remove(this as Cell);
        GameManager.instance.activeCells.RemoveAll(x => x == null);
        cellInWorld.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f, 1);
    }
    public void Move(Vector3 dir)
    {
        bool can = true;

        var nextPos = CalcPos(posInArray + Vector2Int.RoundToInt(dir));
        if (!CheckIsCellImpty(dir))
        {
            if (GameManager.instance.Get(nextPos).isDead == false && kind == CellKind.Sun)
            {
                can = false;
            }
        }

        if (can)
        {
            bool isEated = EatDead(nextPos);
            if (isEated)
            {
                GameManager.instance.Set(CalcPos(posInArray), null);
                GameManager.instance.Set(nextPos, this);
                posInArray = CalcPos(posInArray + Vector2Int.RoundToInt(dir));
                energy -= actionEnergyMove;
                energy += isEated ? maxEnergy / 2f : 0;
            }
        }
    }
    public bool Dublicate(Vector3 dir)
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
            if (isEated)
            {
                var newPos = nextPos;
                var newCell = GameManager.instance.Set(newPos, GameManager.instance.GetFromPool());
                newCell.WorldInit(newPos, this);
                newCell.posInArray = nextPos;
                newCell.energy = world.actionEnergy;
                energy -= world.actionEnergy;
                return true;
            }
        }
        return false;
    }
    public bool EatDead(Vector2Int nextPos)
    {
        var nearCell = GameManager.instance.Get(nextPos);
        if (nearCell != null)
        {
            if (nearCell.isDead)
            {
                hp += 10;
                energy += maxEnergy/2f;
                GameManager.instance.Set(nextPos, null, true);
                return true;
            }
            else if (kind != CellKind.Sun)
            {
                if (nearCell.hp > hp && kind == CellKind.Meat)
                {
                    Death();
                    return false;
                }
                if (IsntBrotherCell(nearCell))
                {
                    hp += nearCell.hp/2f;
                    energy += maxEnergy/2f;
                    GameManager.instance.Set(nextPos, null, true);
                    return true;
                }
            }
        }
        return nearCell == null ? true : false;
    }

    public bool IsntBrotherCell(Cell nearCell)
    {
        return (nearCell.CollectGens() - CollectGens() > 50 || energy < world.maxEnergy / 2f || nearCell.kind != kind) && nearCell.hp < hp;
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
    public CellData GetCellData()
    {
        return new CellData() { currentThought = currentThought, rotation = rotation, energy = energy, kind = kind, thoughts = thoughts, lastAction = lastAction };
    }


    public void print(object obj)
    {
        Debug.Log(obj.ToString());
    }
}
