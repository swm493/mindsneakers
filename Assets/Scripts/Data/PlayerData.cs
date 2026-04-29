using UnityEngine;

[System.Serializable]
public class PlayerData
{
    private int _day;
    public int Day
    {
        get => _day;
        set
        {
            _day = value;
            SaveManager.Instance.SaveGame();
        }
    }
    public int level;
}