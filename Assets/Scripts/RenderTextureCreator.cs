using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureCreator : MonoBehaviour
{
    public Texture2D texture;
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

    private void OnPostRender()
    {
        if (this.enabled)
        {
            if (CameraModes.cameraMode == CameraModes.CameraMode.Infinity)
            {
                var pixelRect = camera.pixelRect;
                Texture2D txt = new Texture2D((int)pixelRect.width, (int)pixelRect.height, TextureFormat.RGB24, false);
                txt.ReadPixels(pixelRect, 0, 0, false);
                txt.filterMode = FilterMode.Point;
                txt.Apply();
                Destroy(texture);
                texture = txt;
            }
        }
        else
        {
            if (texture == null)
            {
                var pixelRect = camera.pixelRect;
                Texture2D txt = new Texture2D((int)pixelRect.width, (int)pixelRect.height, TextureFormat.RGB24, false);
                txt.ReadPixels(pixelRect, 0, 0, false);
                txt.filterMode = FilterMode.Point;
                txt.Apply();
                texture = txt;
            }
        }
    }
}
