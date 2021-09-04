using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DisplayCell))]
public class DisplayCellDataEditor : Editor
{
    public DisplayCell displayCell; 
    private void OnEnable()
    {
        displayCell = (DisplayCell)target;
    }

    public override void OnInspectorGUI()
    {
        displayCell.UpdateDisplay();
        base.OnInspectorGUI();
    }
}
