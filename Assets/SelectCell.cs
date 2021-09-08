using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCell : MonoBehaviour
{
    public Vector2Int point;
    public static Cell selected;
    private void Update()
    {
        if (selected != null)
        {
            if (selected.isDead)
            {
                selected = null;
            }
        }
        if (CameraModes.cameraMode == CameraModes.CameraMode.Clamp)
        {
            point = Vector2Int.RoundToInt(transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            if (point.x <= GameManager.instance.fieldSize - 1 && point.y <= GameManager.instance.fieldSize - 1 && point.x >= 0 && point.y >= 0)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    selected = GameManager.instance.Get(point);
                }
            }
            else
            {
                selected = null;
            }
        }
        else
        {
            selected = null;
        }
    }
}
