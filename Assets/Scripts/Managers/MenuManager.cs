using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject fade;
    private void Start()
    {
        inputField.text = PlayerPrefs.GetInt("Size", 200).ToString();
    }

    public void Play()
    {
        if (!Application.isLoadingLevel)
        {
            fade.SetActive(true);
            StartCoroutine(wait());
        }
    }

    IEnumerator wait()
    {
        yield return new WaitForSecondsRealtime(1);
        SceneManager.LoadSceneAsync(1);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void ChangeSize()
    {
        var val = int.Parse(inputField.text);
        if (val < 20)
        {
            val = 20;
            inputField.text = val.ToString();
        }
        PlayerPrefs.SetInt("Size", val);
    }
}
