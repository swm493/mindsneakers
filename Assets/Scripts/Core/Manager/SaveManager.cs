using UnityEngine;
using System.IO;

public class SaveManager : MonoSingleton<SaveManager>
{
    public PlayerData playerData = new();

    private string saveFilePath;

    protected override void Awake()
    {
        base.Awake();
        saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");
    }

    private void Start()
    {
        if (!File.Exists(saveFilePath))
        {
            ResetData();
        }
        else
        {
            LoadGame();
        }
    }

    public void ResetData()
    {
        playerData = new PlayerData
        {
            day = 0,
            level = 0
        };

        SaveGame();
        ApplyDataToGame();
        MyDebug.Log("데이터 초기화 완료");
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(playerData, true);

        File.WriteAllText(saveFilePath, json);

        MyDebug.Log("저장 완료: " + saveFilePath);
    }

    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            MyDebug.LogError("저장된 파일이 없습니다!");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        playerData = JsonUtility.FromJson<PlayerData>(json);
        ApplyDataToGame();
        MyDebug.Log("로드 완료");
    }

    private void ApplyDataToGame()
    {
        switch (playerData.day)
        {
            case 0:
                playerData.level = 0;
                break;
            case 1:
                playerData.level = 2;
                break;
            case 2:
                playerData.level = 6;
                break;
            default:
                break;
        }
    }
}