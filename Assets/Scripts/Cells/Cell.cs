using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rotation { Left, Up, Right, Down};
public enum CellKind { Sun, Meat, Combined};

[System.Serializable]
public class Cell : CellCore
{
    public Color genColor, typeColor;
    public Cell(Transform cell)
    {
        Init(cell);
    }

    public void Init(Transform cell)
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
        Init(cellInWorld);

        cellInWorld.gameObject.SetActive(true);
        cellInWorld.position = (Vector2)pos;
        posInArray = pos;
        cellBonuses = cellInWorld.GetComponent<CellBonuses>();
        if (parent != null)
        {
            thoughts = (parent as Cell).thoughts;
            if (Random.Range(1, 5) == 1)
            {
                rotation = (parent as Cell).GetCellData().rotation;
            }
            if (Random.Range(0, 100) == 1)
                thoughts[Random.Range(0, thoughtsLength)] = (byte)Random.Range(0, thoughtsMax);

            if (thoughts[0] % 4 == 0)
            {
                energy = world.actionEnergy;
                kind = CellKind.Meat;
            }
            else if (thoughts[0] % 6 == 0)
            {
                kind = CellKind.Combined;
            }
            else
            {
                kind = CellKind.Sun;
            }
        }
        else
        {
            thoughts[0] = 9;
            kind = CellKind.Sun;
        }
        
        

        float r = 0, g = 0, b = 0;
        var thoughtCount = thoughtsLength;
        for (int i = 0; i < thoughtCount; i++)
        {
            if (i < thoughtCount * 0.3f)
            {
                r += thoughts[i];
            }
            else if (i >= thoughtCount * 0.3f && i <= thoughtCount * 0.7f)
            {
                g += thoughts[i];
            }
            else
            {
                b += thoughts[i];
            }
        }
        float max = Mathf.Max(Mathf.Max(r, g), b);
        genColor = new Color32((byte)(((r / max) * (255))), (byte)((g / max) * (255)), (byte)((b / max) * (255)), 255);

        Color.RGBToHSV(genColor, out float h, out float s, out float v);
        genColor = Color.HSVToRGB(h, s * 2f, v);

        typeColor = kind == CellKind.Sun ? Color.green : (kind == CellKind.Meat ? Color.red : Color.blue);
        hp = cellBonuses.GetValue(GetCellData(), GenTypes.HP) * Random.Range(1f, 1.5f);
        maxhp = hp;
        ChangeColor();
    }

    public void ChangeColor()
    {
        if (!isDead)
        {
            cellInWorld.GetComponent<SpriteRenderer>().color = CameraModes.viewMode == CameraModes.ViewMode.Gen ? genColor : typeColor;
        }
    }
    public void UnInit()
    {
        cellInWorld.gameObject.SetActive(false);
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
        else
        {
            if (kind != CellKind.Sun)
            {
                if (thoughts[currentThought] == 2)
                {
                    MoveCell();
                    MoveCell();
                }
            }
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
        if (energy < world.actionEnergy) return;

        if (kind == CellKind.Sun)
        {
            SunCellRotating();
        }
        if (kind == CellKind.Meat)
        {
            var down = GameManager.instance.Get(CalcPos(posInArray + Vector2Int.down));
            var up = GameManager.instance.Get(CalcPos(posInArray + Vector2Int.up));
            var left = GameManager.instance.Get(CalcPos(posInArray + Vector2Int.left));
            var right = GameManager.instance.Get(CalcPos(posInArray + Vector2Int.right));

            bool isFinded = false;

            if (down != null)
            {
                if (IsntBrotherCell(down))
                {
                    rotation = Rotation.Down;
                    isFinded = true;
                }
            }
            if (up != null)
            {
                if (IsntBrotherCell(up))
                {
                    rotation = Rotation.Up;
                    isFinded = true;
                }
            }
            if (left != null)
            {
                if (IsntBrotherCell(left))
                {
                    rotation = Rotation.Left;
                    isFinded = true;
                }
            }
            if (right != null)
            {
                if (IsntBrotherCell(right))
                {
                    rotation = Rotation.Right;
                    isFinded = true;
                }
            }
            if (isFinded == false)
            {
                SunCellRotating();
            }
        }
        lastAction = "Rotate";
    }

    public void SunCellRotating()
    {
        energy -= world.actionEnergy;
        int rotate = (int)rotation;
        rotate += Random.Range((thoughts[6] % 2 == 0 || thoughts[4] % 4 == 0) ? -1 : 0, (thoughts[6] % 5 == 0 || thoughts[7] % 9 == 0) ? 2 : 0);
        if (rotate >= 4) rotate = 0;
        if (rotate < 0) rotate = 3;
        rotation = (Rotation)rotate;
    }

    public void DublicateCell()
    {
        if (energy < world.actionEnergy) return;

        bool isDubed = false;
        switch (rotation)
        {
            case Rotation.Left:
                isDubed = Dublicate(Vector3.left);
                break;
            case Rotation.Up:
                isDubed = Dublicate(Vector3.up);
                break;
            case Rotation.Right:
                isDubed = Dublicate(Vector3.right);
                break;
            case Rotation.Down:
                isDubed = Dublicate(Vector3.down);
                break;
        }
        if (isDubed == false)
        {
            RotateCell();
        }
        lastAction = "Dublicate";
    }
    public void MoveCell()
    {
        if (energy < actionEnergyMove) return;

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
        energy += world.sunEnergy * cellBonuses.GetValue(GetCellData(), GenTypes.SUNBONUS);
        if ((int)energy > maxEnergy)
        {
            energy = maxEnergy;
        }
    }
    public void MeatCellUpdate()
    {
        if ((int)energy > maxEnergy)
        {
            energy = maxEnergy;
        }
        if (energy <= 0) Death();
    }
    public void CombinedCellUpdate()
    {
        SunCellUpdate();
        MeatCellUpdate();
    }


    public new CellData GetCellData()
    {
        return new CellData() { currentThought = currentThought, rotation = rotation, energy = energy, kind = kind, thoughts = thoughts, moveEnergy = actionEnergyMove, genColor = genColor, isDead = isDead, typeColor = typeColor, maxEnergy = maxEnergy, hp = hp, maxHp = maxhp};
    }

}
