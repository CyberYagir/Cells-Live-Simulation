using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TMP_Text sun, meat, combined, tiks; 
    public RenderTextureCreator renderTexture;
    private void Start()
    {
        instance = this;
    }
    public void UpdateUI()
    {
        tiks.text = $"Tick: {GameManager.instance.ticks}";
        sun.text = $"{GameManager.instance.sun} c.";
        meat.text = $"{GameManager.instance.meat} c.";
        combined.text = $"{GameManager.instance.combined} c.";
    }
}
