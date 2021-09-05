using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCell : MonoBehaviour
{
    public CellData cellData;
    public int currentVal;
    public void UpdateDisplay()
    {
        if (Application.isPlaying)
        {
            var newCell = GameManager.instance.Get(new Vector2Int((int)transform.position.x, (int)transform.position.y));
            if (newCell != null)
            {
                cellData = newCell?.GetCellData();
                currentVal = cellData.thoughts[cellData.currentThought];
            }
            
        }
    }
}
