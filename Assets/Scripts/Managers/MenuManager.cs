using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject fade;
    public static WorldObject world;
    private void Awake()
    {
        Time.timeScale = 1;
        world = SaveManager.LoadWorld();
        inputField.text = world.fieldSize.ToString();
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
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

    public void ResetSave()
    {
        SaveManager.DeleteSave();
        Application.LoadLevel(0);
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
        world.fieldSize = val;
        SaveManager.SaveWorld(world);
    }
}
