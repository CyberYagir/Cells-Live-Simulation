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
    [Space(20)]
    public GameObject info;
    public TMP_Text infoText, timeText;
    public Image cell;
    public Slider slider;

    private void Start()
    {
        instance = this;
        slider.value = Time.timeScale;
    }
    private void Update()
    {
        timeText.text = $"Time [{Time.timeScale.ToString("000")}]: ";
    }
    public void ChangeTime()
    {
        Time.timeScale = slider.value;
    }
    public void UpdateUI()
    {
        tiks.text = $"Tick: {GameManager.instance.ticks}";
        sun.text = $"{GameManager.instance.sun} c.";
        meat.text = $"{GameManager.instance.meat} c.";
        combined.text = $"{GameManager.instance.combined} c.";


        if (SelectCell.selected == null)
        {
            info.SetActive(false);
        }
        else
        {
            info.SetActive(true);
            var n = SelectCell.selected.GetCellData();
            cell.color = CameraModes.viewMode == CameraModes.ViewMode.Gen ? n.genColor : n.typeColor;
            cell.rectTransform.localEulerAngles = new Vector3(0, 0, ((int)n.rotation * -90));

            infoText.text =
                $"Rot: {n.rotation}\n" + 
                $"Type: <color=\"" + (n.kind == CellKind.Sun ? "green" : (n.kind == CellKind.Combined ? "blue" : "red")) + $"\"> {n.kind}</color>\n" +
                $"Energy: {(int)n.energy}/{(int)n.maxEnergy}\n" +
                $"HP: {(int)n.hp}/{(int)n.maxHp}\n" +
                $"Move Energy: {n.moveEnergy}\n\n" +
                $"Gens: \n<u>";
            for (int i = 0; i < n.thoughts.Length; i++)
            {
                if (i % 4 == 0)
                {
                    infoText.text += "\n";
                }
                if (i == n.currentThought)
                    infoText.text += "<color=\"yellow\">";

                infoText.text += n.thoughts[i].ToString("00") + "|";


                if (i == n.currentThought)
                    infoText.text += "</color>";
            }
        }
    }
}
