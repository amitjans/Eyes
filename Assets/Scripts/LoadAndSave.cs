using Assets.Scripts;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LoadAndSave
{
    public static void SaveData(Save data)
    {
        if (DataSaved())
        {
            File.Delete(Application.persistentDataPath + "/save.dat");
        }
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
        bf.Serialize(file, data);
        file.Close();
    }

    public static Save LoadData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);
        Save data = (Save)bf.Deserialize(file);
        file.Close();
        return data;
    }

    public static bool DataSaved()
    {
        return File.Exists(Application.persistentDataPath + "/save.dat");
    }
}