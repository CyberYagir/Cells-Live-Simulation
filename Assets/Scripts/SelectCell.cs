using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCell : MonoBehaviour
{
    public Vector2Int point;
    public static Cell selected;
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

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
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                point = Vector2Int.RoundToInt(transform.InverseTransformPoint(camera.ScreenToWorldPoint(Input.mousePosition)));
                if (point.x <= GameManager.Instance.fieldSize - 1 && point.y <= GameManager.Instance.fieldSize - 1 && point.x >= 0 && point.y >= 0)
                {
                    selected = GameManager.Instance.Get(point);
                    if (selected != null)
                    {
                        if (selected.isDead)
                        {
                            selected = null;
                        }
                    }
                }
                else
                {
                    selected = null;
                }
            }
        }
        else
        {
            selected = null;
        }
    }
}
