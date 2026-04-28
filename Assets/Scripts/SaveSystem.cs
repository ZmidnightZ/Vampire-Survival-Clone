using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string savePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments),"MyGame","save.json");
    public static void SaveAll()
    {
        SaveData data = new SaveData();

        // Collect data
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

        // Convert to JSON
        string json = JsonUtility.ToJson(data, true);

        string folder = Path.GetDirectoryName(savePath);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        // Save to file
        File.WriteAllText(savePath, json);

        Debug.Log("Game Saved to: " + savePath);
    }

    public static void LoadAll()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("No save file found");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Apply data
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

        // Update UI
        if (UIController.instance != null)
        {
            UIController.instance.UpdateCoins();
            UIController.instance.UpdateHighScoreDisplay();
        }
    }

    // ---------------- HIGH SCORE ----------------

    private static float highScore = 0f;

    public static float GetHighScore()
    {
        return highScore;
    }

    public static void SetHighScore(float value)
    {
        highScore = value;
    }

    public static void TrySetHighScore(float score)
    {
        if (score > highScore)
        {
            highScore = score;
        }
    }

    // ---------------- RESET ----------------

    public static void ResetData()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        // Reset runtime values
        if (CoinController.instance != null)
            CoinController.instance.currentCoins = 0;

        if (PlayerStatController.instance != null)
        {
            PlayerStatController.instance.moveSpeedLevel = 0;
            PlayerStatController.instance.healthLevel = 0;
            PlayerStatController.instance.pickupRangeLevel = 0;
            PlayerStatController.instance.maxWeaponsLevel = 0;
        }

        highScore = 0;

        if (UIController.instance != null)
        {
            UIController.instance.UpdateCoins();
            UIController.instance.UpdateHighScoreDisplay();
        }

        Debug.Log("Save data reset");
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