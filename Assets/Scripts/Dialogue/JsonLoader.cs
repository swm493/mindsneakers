using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public static class JsonLoader<T>
{
    public static T LoadJson(string filePath)
    {
        string path = Path.Combine(Application.streamingAssetsPath, filePath);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }
        else
        {
            throw new FileNotFoundException($"JSON 파일이 존재하지 않습니다: {path}");
        }
    }
}