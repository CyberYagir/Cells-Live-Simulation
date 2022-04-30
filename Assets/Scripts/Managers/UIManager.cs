using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] TMP_Text sun, meat, combined, tiks; 
    [SerializeField] RenderTextureCreator renderTexture;
    [Space(20)]
    [SerializeField] GameObject info;
    [SerializeField] TMP_Text infoText, timeText;
    [SerializeField] Image cell;
    [SerializeField] Slider slider;

    [SerializeField] TMP_Text buttonModeView, buttonModeCamera;

    [SerializeField] Animator escapeAnimator;
    [SerializeField] GameObject menuBlackScreen;
    bool isEscape;
    [HideInInspector]
    public bool canEscape = true;

    private void Start()
    {
        canEscape = true;
        instance = this;
        slider.value = Time.timeScale;
        UpdateUI();
    }
    public void Escape()
    {
        if (canEscape)
        {
            isEscape = !isEscape;
            canEscape = false;
            escapeAnimator.Play(isEscape ? "MenuIn" : "MenuOut");
        }
    }
    public void ToMenu()
    {
        if (!Application.isLoadingLevel)
        {
            menuBlackScreen.SetActive(true);
            StartCoroutine(toMenuWait());
        }
    }

    IEnumerator toMenuWait()
    {
        yield return new WaitForSecondsRealtime(1);
        Application.LoadLevelAsync(0);
    }
    public void Exit()
    {
        Application.Quit();
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Escape();
        }

        buttonModeView.text = $"View: {CameraModes.viewMode}";
        buttonModeCamera.text = $"Map: {CameraModes.cameraMode}";
        timeText.text = $"Time [{Time.timeScale.ToString("000")}]: ";
    }
    public void ChangeTime()
    {
        Time.timeScale = slider.value;
    }
    public void UpdateUI()
    {
        tiks.text = $"Tick: {GameManager.Instance.ticks} [{GameManager.Instance.generation}]";
        sun.text = $"{GameManager.Instance.sun} c.";
        meat.text = $"{GameManager.Instance.meat} c.";
        combined.text = $"{GameManager.Instance.combined} c.";


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
