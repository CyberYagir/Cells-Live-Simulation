using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rotation { Left, Up, Right, Down};
public enum CellKind { Sun, Meat, Combined};

public enum GenType
{
    SunGen = 5,
    HealthGen = 6,
    TypeGen = 0
}

[System.Serializable]
public class Cell : CellCore
{
    public Color genColor, typeColor;
    public Cell(Transform cell)
    {
        uid = Random.Range(int.MinValue, int.MaxValue);
        Init(cell);
    }

    public void Init(Transform cell, bool initThouths = true)
    {
        cellInWorld = cell;
        cellInWorld.GetComponent<DisplayCell>().cell = this;
        cellInWorld.gameObject.SetActive(false);
        
        if (smoothMover == null)
        {
            smoothMover = cellInWorld.GetComponent<SmoothMover>();
        }

        if (renderer == null)
        {
            renderer = cellInWorld.GetComponent<SpriteRenderer>();
        }

        if (initThouths)
        {
            thoughts = new byte[thoughtsLength];
            for (int i = 0; i < thoughts.Length; i++)
            {
                thoughts[i] = (byte) Random.Range(1, thoughtsMax);
                if (thoughts[i] == thoughts.Length)
                {
                    thoughts[i]++;
                }
            }

            genSum = CollectGens();
        }

        isDead = false;
        currentThought = 0;
        rotation = (Rotation) Random.Range(0, 4);
        world = GameManager.Instance.world;
    }

    public void WorldInit(Vector2Int pos, CellCore parent = null)
    {
        Init(cellInWorld, parent == null);
        cellInWorld.gameObject.SetActive(true);
        cellInWorld.position = (Vector2)pos;
        smoothMover.newPos = ((Vector2)pos);
        posInArray = pos;
        if (parent != null)
        {
            var parentCell = (parent as Cell);
            thoughts = parentCell.thoughts;
               
            if (Random.Range(0, thoughts[thoughtsLength-1] * world.duplicateRarity) == 0)
                thoughts[Random.Range(0, thoughtsLength)] = (byte)Random.Range(0, thoughtsMax);
            genSum = CollectGens();
            kind = parent.kind;
            
            var dirs = new List<bool>();
            dirs.Add(CheckDir(Vector2Int.up));
            dirs.Add(CheckDir(Vector2Int.down));
            dirs.Add(CheckDir(Vector2Int.left));
            dirs.Add(CheckDir(Vector2Int.right));

            if (dirs.FindAll(x=>x == true).Count <= 2)
            {
                if (thoughts[(int)GenType.TypeGen] % 3 == 0)
                {
                    energy = world.maxEnergy;
                    kind = CellKind.Meat;
                }
                else if (thoughts[(int)GenType.TypeGen] % 5 == 0)
                {
                    energy = world.maxEnergy / 2f;
                    kind = CellKind.Combined;
                }
                else
                {
                    kind = CellKind.Sun;
                }
            }
        }
        
        CalcGenColor();

        typeColor = kind == CellKind.Sun ? Color.green : (kind == CellKind.Meat ? Color.red : Color.blue);
        
        hp = thoughts[(int)GenType.HealthGen] * world.heathMultiply  * world.healthRandom.GetRandom();
        
        maxhp = hp;
        ChangeColor();
    }


    public void CalcGenColor()
    {
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

    }
    
    public void ChangeColor()
    {
        if (!isDead)
        {
            if (CameraModes.viewMode == CameraModes.ViewMode.Energy)
            {
                EnergyColorSet();
            }
            else
            {
                renderer.color = CameraModes.viewMode == CameraModes.ViewMode.Gen ? genColor : typeColor;
            }
        }
    }
    public void UnInit()
    {
        cellInWorld.gameObject.SetActive(false);
    }

    public void Brain()
    {
        if (thoughts[currentThought] == 4 || thoughts[currentThought] == 5)
        {
            DublicateCell();
        }
        else
        if (thoughts[currentThought] == 6)
        {
            MoveCell();
        }
        else
        if (thoughts[currentThought] == 9)
        {
            RotateCell();
        }
        else if (thoughts[currentThought] == 8 && world.isEnableTransfer)
        {
            TransferCellEnergy();
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
    
    public void TransferCellEnergy()
    {
        if (energy >= world.maxEnergy-1)
        {
            switch (rotation)
            {
                case Rotation.Left:
                    TransferEnergy(Vector2Int.left);
                    break;
                case Rotation.Up:
                    TransferEnergy(Vector2Int.up);
                    break;
                case Rotation.Right:
                    TransferEnergy(Vector2Int.right);
                    break;
                case Rotation.Down:
                    TransferEnergy(Vector2Int.down);
                    break;
            }
            
        }
    }

    

    public bool CheckDir(Vector2Int dir)
    {
        var cell = GameManager.Instance.Get(CalcPos(posInArray + dir));
        if (cell != null)
        {
            if (cell.isDead)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    public void RotateCell()
    {
        if (energy < actionEnergy/2f) return;

        var down = GameManager.Instance.Get(CalcPos(posInArray + Vector2Int.down));
        var up = GameManager.Instance.Get(CalcPos(posInArray + Vector2Int.up));
        var left = GameManager.Instance.Get(CalcPos(posInArray + Vector2Int.left));
        var right = GameManager.Instance.Get(CalcPos(posInArray + Vector2Int.right));

        bool isFinded = false;
        if (kind == CellKind.Sun)
        {
            SunCellRotating();
        }else
        if (kind == CellKind.Meat)
        {
            if (down != null)
            {
                if (IsNotBrotherCell(down))
                {
                    rotation = Rotation.Down;
                    isFinded = true;
                }
            }
            if (up != null)
            {
                if (IsNotBrotherCell(up))
                {
                    rotation = Rotation.Up;
                    isFinded = true;
                }
            }
            if (left != null)
            {
                if (IsNotBrotherCell(left))
                {
                    rotation = Rotation.Left;
                    isFinded = true;
                }
            }
            if (right != null)
            {
                if (IsNotBrotherCell(right))
                {
                    rotation = Rotation.Right;
                    isFinded = true;
                }
            }
            if (isFinded == false)
            {
                SunCellRotating();
            }
            else
            {
                energy -= actionEnergy / 2f;
            }
        }
    }

    public Rotation GetInverseRotation()
    {
        var rot = (int)rotation;
        for (int i = 0; i < 2; i++)
        {
            rot++;
            if (rot >= 4)
            {
                rot = 0;
            }
        }
        return (Rotation)rot;
    }
    public Vector3 RotationToVector(Rotation rot)
    {
        switch (rot)
        {
            case Rotation.Left:
                return Vector3.left;
            case Rotation.Up:
                return Vector3.up;
            case Rotation.Right:
                return Vector3.right;
            case Rotation.Down:
                return Vector3.down;
        }
        return new Vector3();
    }

    public void SunCellRotating()
    {
        energy -= actionEnergy/2f;
        int rotate = (int)rotation;
        rotate += Random.Range((thoughts[6] % 2 == 0 || thoughts[4] % 4 == 0) ? -1 : 0, (thoughts[6] % 5 == 0 || thoughts[7] % 9 == 0) ? 2 : 0);
        if (rotate >= 4) rotate = 0;
        if (rotate < 0) rotate = 3;
        rotation = (Rotation)rotate;
    }

    public void DublicateCell()
    {
        if (energy < actionEnergy) return;

        bool isDubed = false;
        switch (rotation)
        {
            case Rotation.Left:
                isDubed = Duplicate(Vector3.left);
                break;
            case Rotation.Up:
                isDubed = Duplicate(Vector3.up);
                break;
            case Rotation.Right:
                isDubed = Duplicate(Vector3.right);
                break;
            case Rotation.Down:
                isDubed = Duplicate(Vector3.down);
                break;
        }
        if (isDubed == false)
        {
            RotateCell();
        }
    }
    public void MoveCell()
    {
        if (energy < actionEnergy) return;
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
    }


    public void UpdateCell()
    {
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
                    CombinedCellUpdate();
                    break;
            }
            var oldPos = (Vector2)posInArray;
            Brain();
            
            SmoothMove(oldPos);

            EnergyColorSet();
        }
        hp -= world.deathSpeed;
        if (hp <= 0)
        {
            Death();
        }
    }

    public void EnergyColorSet()
    {
        if (!isDead && CameraModes.viewMode == CameraModes.ViewMode.Energy)
        {
            renderer.color = new Color(energy/world.maxEnergy, 0, 0, 1);
        }
    }

    public void SmoothMove(Vector2 oldPos)
    {
        if (GameManager.Instance.activeCells.Count < 1000)
        {
            smoothMover.enabled = true;
            if (Vector2.Distance(posInArray, oldPos) > 5)
            {
                cellInWorld.position = (Vector2)posInArray;
            }
            smoothMover.newPos = (Vector2)posInArray;
        }
        else
        {
            smoothMover.enabled = false;
            cellInWorld.position = (Vector2)posInArray;
        }
    }
    
    public void SunCellUpdate()
    {
        energy += world.sunEnergy + (thoughts[(int)GenType.SunGen]/world.sunGenDevider);
        if ((int)energy > world.maxEnergy)
        {
            energy = world.maxEnergy;
        }
        if ((int)hp > maxhp)
        {
            hp = maxhp;
        }
    }
    public void MeatCellUpdate()
    {
        if ((int)energy > world.maxEnergy)
        {
            energy = world.maxEnergy;
        }
        else
        {
            if (kind == CellKind.Meat)
            {
                energy += world.predatorTimeBoost;
            }
        }

        if ((int)hp > maxhp)
        {
            hp = maxhp;
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
        return new CellData() { currentThought = currentThought, rotation = rotation, energy = energy, kind = kind, thoughts = thoughts, moveEnergy = actionEnergy, genColor = genColor, isDead = isDead, typeColor = typeColor, maxEnergy = world.maxEnergy, hp = hp, maxHp = maxhp};
    }

}
