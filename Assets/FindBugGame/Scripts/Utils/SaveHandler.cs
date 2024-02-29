using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveHandler
{
    public void SaveData(object objectTosave, string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName + ".bin";
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(filePath, FileMode.Create);
        formatter.Serialize(fileStream, objectTosave);
        fileStream.Close();
    }

    public object LoadData(string fileName)
    {
        string filePath = Application.persistentDataPath + "/" + fileName + ".bin";
        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            object obj = formatter.Deserialize(fileStream);
            fileStream.Close();
            return obj;
        }
        else
        {
            return null;
        }
    }
}
