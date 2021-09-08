using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraModes : MonoBehaviour
{
    public enum CameraMode {Infinity, Clamp };
    public static CameraMode cameraMode;


    public enum ViewMode { Gen, Type};
    public static ViewMode viewMode;

    public RenderTextureCreator renderTexture;
    public RawImage worldUI;

    private void Start()
    {
        SetCameraMode();
    }

    public void SetViewMode()
    {
        if (viewMode == ViewMode.Gen) viewMode = ViewMode.Type; else viewMode = ViewMode.Gen;

        foreach (var item in GameManager.instance.activeCells)
        {
            item.ChangeColor();
        }
    }

    public void SetCameraMode()
    {
        if (cameraMode == CameraMode.Clamp)
        {
            cameraMode = CameraMode.Infinity;
            renderTexture.enabled = true;
            worldUI.gameObject.SetActive(true);
            renderTexture.GetComponent<Camera>().depth = -1f;
        }
        else
        {
            worldUI.gameObject.SetActive(false);
            renderTexture.enabled = false;
            renderTexture.GetComponent<Camera>().rect = new Rect(new Vector2(0, 0.09f), new Vector2(1, 0.91f));
            renderTexture.GetComponent<Camera>().depth = -0.9f;
            cameraMode = CameraMode.Clamp;
        }
    }
}