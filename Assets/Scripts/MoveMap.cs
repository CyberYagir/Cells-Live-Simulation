using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveMap : MonoBehaviour
{
    public Vector2 offcet;
    public float speed, scaleSpeed;
    RenderTextureCreator rtc;
    public float scale = 1;
    [SerializeField] Vector2 offcetRect;
    [SerializeField] Vector2 sizeRect;

    RawImage image;

    public float a = 0;
    private void Start()
    {
        image = GetComponent<RawImage>();
        rtc = FindObjectOfType<RenderTextureCreator>();
    }
    void Update()
    {
        a = Mathf.Clamp01(a); 
        image.color = new Color(1, 1, 1, a);
        if (rtc.texture != null)
        {
            image.texture = rtc.texture;
        }
        if (CameraModes.cameraMode == CameraModes.CameraMode.Infinity)
        {
            a += Time.unscaledDeltaTime * 2;
            if (Input.GetKey(KeyCode.Mouse1))
            {
                scale += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * Time.unscaledDeltaTime * scaleSpeed;
                offcet -= Vector2.one * Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * Time.unscaledDeltaTime * scaleSpeed;
            }
            scale = Mathf.Clamp(scale, 0.5f, 5);
            if (Input.GetKey(KeyCode.Mouse0))
            {
                offcet -= new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.unscaledDeltaTime * speed * scale;
            }
            
            offcetRect = Vector2.Lerp(offcetRect, offcet, 10 * Time.unscaledDeltaTime);
            sizeRect = Vector2.Lerp(sizeRect, new Vector2(1f * ((float)Screen.width / (float)Screen.height), 1) * scale, 10 * Time.unscaledDeltaTime);

            image.uvRect = new Rect(offcetRect, sizeRect);
        }
        else
        {
            a -= Time.unscaledDeltaTime * 2;
        }
    }
}
