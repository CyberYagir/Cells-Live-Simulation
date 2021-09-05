using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureCreator : MonoBehaviour
{
    public int height;
    public int oldHeight;
    public Texture2D texture;

    private void Update()
    {
        Camera.main.pixelRect = new Rect(Vector2.zero, new Vector2(Screen.height, Screen.height));
    }
    private void OnPostRender()
    {
        Texture2D txt = new Texture2D((int)Camera.main.pixelRect.width, (int)Camera.main.pixelRect.height, TextureFormat.RGB24, false);
        txt.ReadPixels(Camera.main.pixelRect, 0, 0, false);
        txt.filterMode = FilterMode.Point;
        txt.Apply();
        Destroy(texture);
        texture = txt;
    }
}
