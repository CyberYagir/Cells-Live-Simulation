using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
[System.Serializable]
public class SaveManager : MonoBehaviour
{

    public static string SavesFolder = Directory.GetParent(Application.dataPath) + "/Saves/";
    public static string SaveFile = SavesFolder + "World.json";
    
    public static void SaveWorld(WorldObject world)
    {
        if (!Directory.Exists(SavesFolder))
        {
            Directory.CreateDirectory(SavesFolder);
        }
        File.WriteAllText(SaveFile, JsonUtility.ToJson(world));
    }

    public static WorldObject LoadWorld()
    {
        try
        {
            if (File.Exists(SaveFile))
            {
                var newWorld = ScriptableObject.CreateInstance<WorldObject>();
                JsonUtility.FromJsonOverwrite(File.ReadAllText(SaveFile), newWorld);
                return newWorld;
            }
        }
        finally {}
        return ScriptableObject.CreateInstance<WorldObject>();
    }

    public static void DeleteSave()
    {
        if (File.Exists(SaveFile))
        {
            File.Delete(SaveFile);
        }
    }
}
