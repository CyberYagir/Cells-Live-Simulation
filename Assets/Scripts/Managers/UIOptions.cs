using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [SerializeField] private List<WorldObject> worldObjects;
    public TMP_Dropdown drop;
    public TMP_InputField sunEnergy;
    public TMP_InputField maxEnergy;
    public TMP_InputField actionEnergy;
    public TMP_InputField deathSpeed;
    public TMP_InputField heathMultiply;
    public TMP_InputField duplicateRarity;
    public TMP_InputField sunGenDevider;
    public TMP_InputField predatorTimeBoost;
    public TMP_InputField isBrotherDifference;
    public TMP_InputField startCount;
    public Toggle enableTransfer;
    

    private void Start()
    {
        worldObjects.Insert(0, MenuManager.world);
        
        sunEnergy.text = MenuManager.world.sunEnergy.ToString();
        maxEnergy.text = MenuManager.world.maxEnergy.ToString();
        actionEnergy.text = MenuManager.world.actionEnergy.ToString();
        deathSpeed.text = MenuManager.world.deathSpeed.ToString();
        heathMultiply.text = MenuManager.world.heathMultiply.ToString();
        duplicateRarity.text = MenuManager.world.duplicateRarity.ToString();
        sunGenDevider.text = MenuManager.world.sunGenDevider.ToString();
        predatorTimeBoost.text = MenuManager.world.predatorTimeBoost.ToString();
        isBrotherDifference.text = MenuManager.world.isBrotherDifference.ToString();
        startCount.text = MenuManager.world.startCount.ToString();
        enableTransfer.isOn = MenuManager.world.isEnableTransfer;
    }

    public void ChangeSunEnergy()
    {
        MenuManager.world.sunEnergy = float.Parse(sunEnergy.text);
        
        SaveManager.SaveWorld(MenuManager.world);
    }
    
    public void ChangeMaxEnergy()
    {
        MenuManager.world.maxEnergy = int.Parse(maxEnergy.text);
        SaveManager.SaveWorld(MenuManager.world);
    }
    
    public void ChangeActionEnergy()
    {
        MenuManager.world.actionEnergy = float.Parse(actionEnergy.text);
        SaveManager.SaveWorld(MenuManager.world);
    }
    
    public void ChangeDeathSpeed()
    {
        MenuManager.world.deathSpeed = float.Parse(deathSpeed.text);
        SaveManager.SaveWorld(MenuManager.world);
    }
    
    public void ChangeHeathMultiply()
    {
        MenuManager.world.heathMultiply = int.Parse(heathMultiply.text);
        SaveManager.SaveWorld(MenuManager.world);
    }
    
    public void ChangeDupRarity()
    {
        MenuManager.world.duplicateRarity = int.Parse(duplicateRarity.text);
        SaveManager.SaveWorld(MenuManager.world);
    }
    
    public void ChangeSunGenDevider()
    {
        MenuManager.world.sunGenDevider = float.Parse(sunGenDevider.text);
        SaveManager.SaveWorld(MenuManager.world);
    }
    
    public void ChangePredatorBoost()
    {
        MenuManager.world.predatorTimeBoost = float.Parse(predatorTimeBoost.text);
        SaveManager.SaveWorld(MenuManager.world);
    }
    
    public void ChangeBrotherDif()
    {
        MenuManager.world.isBrotherDifference = int.Parse(isBrotherDifference.text);
        SaveManager.SaveWorld(MenuManager.world);
    }
    
    public void ChangeStartCellsCount()
    {
        MenuManager.world.startCount = int.Parse(startCount.text);
        SaveManager.SaveWorld(MenuManager.world);
    }

    public void ChangeTransfer()
    {
        MenuManager.world.isEnableTransfer = enableTransfer.isOn;
        SaveManager.SaveWorld(MenuManager.world);
    }

    public void SavePresetWorld()
    {
        SaveManager.SaveWorld(worldObjects[drop.value]);
        Application.LoadLevel(0);
    }
}
