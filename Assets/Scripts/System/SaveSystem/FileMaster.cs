using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class FileMaster
{
    public string[] GetFiles(string path, string pattern)//"*.sv"
    {       
        return Directory.GetFiles(path, pattern,SearchOption.TopDirectoryOnly);
    }
    public void CreateDirectory(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public void WriteTo(string path, string text)
    {       
        FileStream fileStream;
        if (File.Exists(path))
            File.Delete(path);
        else
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        fileStream = File.Create(path);
        StreamWriter streamWriter = new StreamWriter(fileStream);
        streamWriter.Write(text);
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("Text Saved To: " + path);
    }
    public string ReadFrom (string path)
    {
        FileStream fileStream;
        if (File.Exists(path))
            fileStream = File.OpenRead(path);
        else
        {
            Debug.LogError("File (" + path + ") not found");
            return default;
        }
        StreamReader streamReader = new StreamReader(fileStream);
        string text = streamReader.ReadToEnd();
        streamReader.Close();
        Debug.Log("Text Loaded From: " + path);
        return text;
    }

    public void WriteTo<T>(string path, T data)
    {
        FileStream fileStream;
        if (File.Exists(path))
            File.Delete(path);
        else
        {
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
        fileStream = File.Create(path);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, data);
        fileStream.Close();
        Debug.Log(typeof(T).Name + " Saved To: " + path);
    }
    public T ReadFrom<T>(string path)
    {
        FileStream fileStream;
        if (File.Exists(path))
            fileStream = File.OpenRead(path);
        else
        {
            Debug.LogError("File ("+ path + ") not found");
            return default;
        }
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        T data = (T)binaryFormatter.Deserialize(fileStream);
        fileStream.Close();

        Debug.Log(typeof(T).Name + " Loaded From: " + path);
        return data;
    }
}

