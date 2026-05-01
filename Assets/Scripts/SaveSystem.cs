using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string savePath =
        Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),
        "MyGame", "save.json");

    // ---------------- SAVE ----------------
    public static void SaveAll()
    {
        SaveData data = new SaveData();

        if (CoinController.instance != null)
            data.coins = CoinController.instance.currentCoins;

        if (PlayerStatController.instance != null)
        {
            data.moveSpeedLevel = PlayerStatController.instance.moveSpeedLevel;
            data.healthLevel = PlayerStatController.instance.healthLevel;
            data.pickupRangeLevel = PlayerStatController.instance.pickupRangeLevel;
            data.maxWeaponsLevel = PlayerStatController.instance.maxWeaponsLevel;
        }

        data.highScore = GetHighScore();

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(savePath, json);
    }

    // ---------------- LOAD ----------------
    public static void LoadAll()
    {
        if (!File.Exists(savePath))
            return;

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (CoinController.instance != null)
            CoinController.instance.currentCoins = data.coins;

        if (PlayerStatController.instance != null)
        {
            PlayerStatController.instance.moveSpeedLevel = data.moveSpeedLevel;
            PlayerStatController.instance.healthLevel = data.healthLevel;
            PlayerStatController.instance.pickupRangeLevel = data.pickupRangeLevel;
            PlayerStatController.instance.maxWeaponsLevel = data.maxWeaponsLevel;
        }

        SetHighScore(data.highScore);
    }

    // ---------------- HIGH SCORE ----------------
    private static float highScore = 0f;

    public static float GetHighScore() => highScore;

    public static void SetHighScore(float value)
    {
        highScore = value;
    }

    public static void TrySetHighScore(float score)
    {
        if (score > highScore)
            highScore = score;
    }
}

[System.Serializable]
public class SaveData
{
    public int coins;

    public int moveSpeedLevel;
    public int healthLevel;
    public int pickupRangeLevel;
    public int maxWeaponsLevel;

    public float highScore;
}