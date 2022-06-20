using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem : MonoBehaviour
{
    private static SaveData _saveData;
    public static SaveData SaveData => _saveData;

    public static void SaveGame()
    {
        Debug.Log("Игра сохранена!");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/saveData.sd";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData LoadData()
    {
        string path = Application.persistentDataPath + "/saveData.sd";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            _saveData = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return _saveData;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    //public static void ClearSaveData()
    //{
    //    _saveData = null;
    //}
}
