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
    public MoveMap worldUI;
    public Vector3 pos;
    public Vector3 size;
     

    private void Start()
    {
        SetCameraMode();
    }

    public void SetViewMode()
    {
        if (viewMode == ViewMode.Gen) viewMode = ViewMode.Type; else viewMode = ViewMode.Gen;

        foreach (var item in GameManager.Instance.activeCells)
        {
            item.ChangeColor();
        }
    }
    private void Update()
    {
        if (cameraMode != CameraMode.Clamp)
        {
            renderTexture.enabled = true;
            renderTexture.GetComponent<Camera>().depth = -1f;
            var rect = renderTexture.GetComponent<Camera>().pixelRect;
            renderTexture.GetComponent<Camera>().pixelRect = new Rect(Vector2.Lerp(rect.position, Vector2.zero, 10 * Time.unscaledDeltaTime), Vector2.Lerp(rect.size, new Vector2(Screen.height, Screen.height), 10 * Time.unscaledDeltaTime));

        }
        else
        {
            renderTexture.enabled = false;
            var rect = renderTexture.GetComponent<Camera>().rect;

            renderTexture.GetComponent<Camera>().rect = new Rect(Vector2.Lerp(rect.position, new Vector2(0, 0.08f), 10 * Time.unscaledDeltaTime), Vector2.Lerp(rect.size, new Vector2(1, 1), 10 * Time.unscaledDeltaTime));
            renderTexture.GetComponent<Camera>().depth = -0.9f;
        }
    }
    public void SetCameraMode()
    {
        if (cameraMode == CameraMode.Clamp)
        {
            cameraMode = CameraMode.Infinity;
        }
        else
        {
            cameraMode = CameraMode.Clamp;
        }
    }
}