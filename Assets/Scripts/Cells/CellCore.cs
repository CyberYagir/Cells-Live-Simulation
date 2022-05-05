using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public abstract class CellCore
{

    public int uid;
    
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


    protected WorldObject world;
    public Vector2Int posInArray;
    public bool isDead;

    protected SmoothMover smoothMover;
    public SpriteRenderer renderer;

    public int genSum;
    
    protected float actionEnergy {
        get
        {
            var devider = world.kindsEnergyDeviders.sun;
            if (kind == CellKind.Meat)
            {
                devider = world.kindsEnergyDeviders.meat;
            }else if (kind == CellKind.Combined)
            {
                devider = world.kindsEnergyDeviders.combined;
            }
            
            return (world.actionEnergy / devider);
        }
    }
    
    protected void Death()
    {
        isDead = true;
        renderer.color = world.deathColor;
    }
    protected void TransferEnergy(Vector2Int dir)
    {
        var nextPos = CalcPos(posInArray + dir);
        if (!CheckIsCellEmpty((Vector2)dir))
        {
            var cell = GameManager.Instance.Get(nextPos);
            if (cell.energy < actionEnergy)
            {
                if (!IsNotBrotherCell(cell))
                {
                    cell.energy += actionEnergy;
                    energy -= actionEnergy;
                }
            }
        }
    }
    protected void Move(Vector3 dir)
    {
        if (energy > actionEnergy)
        {
            var nextPos = CalcPos(posInArray + Vector2Int.RoundToInt(dir));
            if (CheckIsCellEmpty(dir) || Eat(dir))
            {
                GameManager.Instance.Set(nextPos, this, false);
                GameManager.Instance.Set(CalcPos(posInArray), null, false, false);
                posInArray = nextPos;
                energy -= actionEnergy;
            }
        }
    }

    private bool Eat(Vector3 dir)
    {
        var nextPos = CalcPos(posInArray + Vector2Int.RoundToInt(dir));
        var nearCell = GameManager.Instance.Get(nextPos);
        if (nearCell != null)
        {
            if (kind == CellKind.Sun)
            {
                if (nearCell.isDead)
                {
                    GameManager.Instance.Set(nextPos, null, true, true);
                    energy += actionEnergy;
                    return true;
                }
            }
            else
            {
                if (nearCell.isDead)
                {
                    GameManager.Instance.Set(nextPos, null, true, true);
                    energy += actionEnergy;
                    return true;
                }
                else
                {
                    if (!IsNotBrotherCell(nearCell))
                    {
                        energy += actionEnergy + nearCell.energy;
                        nearCell.isDead = true;
                        GameManager.Instance.Set(nextPos, null, true, true);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    protected bool Duplicate(Vector3 dir)
    {
        var newPos = CalcPos(posInArray + Vector2Int.RoundToInt(dir));
        if (CheckIsCellEmpty(dir))
        {
            var newCell = GameManager.Instance.Set(newPos, GameManager.Instance.GetFromPool());
            newCell.WorldInit(newPos, this);
            newCell.posInArray = newPos;
            newCell.energy = actionEnergy;
            energy -= actionEnergy;
            return true;
        }
        else
        {
            if (GameManager.Instance.Get(newPos).isDead)
            {
                Move(dir);
                return false;
            }
        }
        return false;
    }

    protected bool IsNotBrotherCell(Cell nearCell)
    {
        return (Mathf.Abs(nearCell.genSum - genSum) > world.isBrotherDifference);
    }

    protected int CollectGens()
    {
        return thoughts.Sum(x => x);
    }
    public bool CheckIsCellEmpty(Vector3 direction)
    {
        return GameManager.Instance.Get(CalcPos(posInArray + Vector2Int.RoundToInt(direction))) == null;
    }
    public Vector2Int CalcPos(Vector2Int pos)
    {
        var newPos = pos;
        if (pos.y >= GameManager.Instance.fieldSize)
        {
            newPos = new Vector2Int(newPos.x, 0);
        }
        if (pos.x >= GameManager.Instance.fieldSize)
        {
            newPos = new Vector2Int(0, newPos.y);
        }

        if (pos.y < 0)
        {
            newPos = new Vector2Int(newPos.x, GameManager.Instance.fieldSize - 1);
        }
        if (pos.x < 0)
        {
            newPos = new Vector2Int(GameManager.Instance.fieldSize - 1, newPos.y);
        }
        return newPos;
    }
   


    public void print(object obj)
    {
        Debug.Log(obj.ToString());
    }
}
