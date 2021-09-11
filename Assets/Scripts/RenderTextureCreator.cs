using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureCreator : MonoBehaviour
{
    public int height;
    public int oldHeight;
    public Texture2D texture;

    private void OnPostRender()
    {
        if (this.enabled)
        {
            if (CameraModes.cameraMode == CameraModes.CameraMode.Infinity)
            {
                Texture2D txt = new Texture2D((int)Camera.main.pixelRect.width, (int)Camera.main.pixelRect.height, TextureFormat.RGB24, false);
                txt.ReadPixels(Camera.main.pixelRect, 0, 0, false);
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
                Texture2D txt = new Texture2D((int)Camera.main.pixelRect.width, (int)Camera.main.pixelRect.height, TextureFormat.RGB24, false);
                txt.ReadPixels(Camera.main.pixelRect, 0, 0, false);
                txt.filterMode = FilterMode.Point;
                txt.Apply();
                texture = txt;
            }
        }
    }
}
