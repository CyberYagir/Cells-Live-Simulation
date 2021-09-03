using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rotation { Left, Up, Right, Down};
public enum CellKind { Sun, Meat, Combined};
public class Cell
{
    Rotation rotation;
    byte[] thoughts;
    byte currentThought = 0;
    byte thoughtsLength = 10;
    Transform cellInWorld;
    CellKind kind;
    float energy;
    World world;
    public Cell(Transform cell, bool isRandomRotation = false, CellKind kind = CellKind.Sun)
    {
        this.kind = kind;
        cellInWorld = cell;
        cell.gameObject.SetActive(false);

        thoughts = new byte[thoughtsLength];
        for (int i = 0; i < thoughts.Length; i++)
        {
            thoughts[i] = (byte)Random.Range(0, 28);
        }
        currentThought = 0;
        if (isRandomRotation) rotation = (Rotation)Random.Range(0, 4);

        world = GameManager.instance.world;
    }

    public void WorldInit(Vector2Int pos)
    {
        cellInWorld.gameObject.SetActive(true);
        cellInWorld.position = (Vector2)pos;
    }


    public void Brain()
    {
        if ((int)thoughts[currentThought] % 5 == 0)
        {
            DublicateCell();
        }else if ((int)thoughts[currentThought] % 2 == 0)
        {
            MoveCell();
        }
        else if ((int)thoughts[currentThought] % 3 == 0)
        {
            RotateCell();
        }

        GoToNextThought();
    }

    public void GoToNextThought()
    {
        var n = thoughts[currentThought];
        for (int i = 0; i < n; i++)
        {
            currentThought++;
            if (currentThought > thoughtsLength)
            {
                currentThought = 0;
            }
        }
    }

    public void RotateCell()
    {

    }
    public void DublicateCell()
    {

    }
    public void MoveCell()
    {

    }


    public void UpdateCell()
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
        Brain();
    }
    public void SunCellUpdate()
    {
        energy += world.sunEnergy;
        if ((int)energy > world.sunEnergy)
        {
            energy = world.sunEnergy;
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
