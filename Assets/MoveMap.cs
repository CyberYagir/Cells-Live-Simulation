using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveMap : MonoBehaviour
{
    public Vector2 offcet;
    public float speed, scaleSpeed;
    RenderTextureCreator rtc;
    float scale = 1;
    private void Start()
    {
        rtc = FindObjectOfType<RenderTextureCreator>();
    }
    void Update()
    {
        scale = Mathf.Clamp(scale, 0.1f, 5);
        if (Input.GetKey(KeyCode.Mouse1))
        {
            if (scale + Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * Time.deltaTime * scaleSpeed < 5)
            {
                scale += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * Time.deltaTime * scaleSpeed;
                offcet -= new Vector2(1, 0.5f) * Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * Time.deltaTime * scaleSpeed;
            }
        }
        else
        if (Input.GetKey(KeyCode.Mouse0))
            offcet -= new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Time.deltaTime * speed;
        if (rtc.texture != null)
        {
            GetComponent<RawImage>().texture = rtc.texture;
        }
        GetComponent<RawImage>().uvRect = new Rect(offcet, new Vector2(1f * ((float)Screen.width / (float)Screen.height), 1) * scale);
    }
}
