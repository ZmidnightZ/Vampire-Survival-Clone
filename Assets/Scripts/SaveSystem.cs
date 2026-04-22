using UnityEngine;

public static class SaveSystem
{
    private const string CoinsKey = "coins";
    private const string MoveSpeedKey = "moveSpeedLevel";
    private const string HealthKey = "healthLevel";
    private const string PickupKey = "pickupRangeLevel";
    private const string MaxWeaponsKey = "maxWeaponsLevel";
    private const string HighScoreKey = "highScore";

    public static void SaveAll()
    {
        if (CoinController.instance != null)
        {
            PlayerPrefs.SetInt(CoinsKey, CoinController.instance.currentCoins);
        }

        if (PlayerStatController.instance != null)
        {
            PlayerPrefs.SetInt(MoveSpeedKey, PlayerStatController.instance.moveSpeedLevel);
            PlayerPrefs.SetInt(HealthKey, PlayerStatController.instance.healthLevel);
            PlayerPrefs.SetInt(PickupKey, PlayerStatController.instance.pickupRangeLevel);
            PlayerPrefs.SetInt(MaxWeaponsKey, PlayerStatController.instance.maxWeaponsLevel);
        }

        PlayerPrefs.Save();
    }

    public static void LoadStats()
    {
        if (PlayerPrefs.HasKey(MoveSpeedKey) && PlayerStatController.instance != null)
        {
            PlayerStatController.instance.moveSpeedLevel = PlayerPrefs.GetInt(MoveSpeedKey);
        }

        if (PlayerPrefs.HasKey(HealthKey) && PlayerStatController.instance != null)
        {
            PlayerStatController.instance.healthLevel = PlayerPrefs.GetInt(HealthKey);
        }

        if (PlayerPrefs.HasKey(PickupKey) && PlayerStatController.instance != null)
        {
            PlayerStatController.instance.pickupRangeLevel = PlayerPrefs.GetInt(PickupKey);
        }

        if (PlayerPrefs.HasKey(MaxWeaponsKey) && PlayerStatController.instance != null)
        {
            PlayerStatController.instance.maxWeaponsLevel = PlayerPrefs.GetInt(MaxWeaponsKey);
        }
    }

    public static void LoadCoins()
    {
        if (CoinController.instance != null)
        {
            if (PlayerPrefs.HasKey(CoinsKey))
            {
                CoinController.instance.currentCoins = PlayerPrefs.GetInt(CoinsKey);
            }
            else
            {
                CoinController.instance.currentCoins = 0;
            }

            if (UIController.instance != null)
            {
                UIController.instance.UpdateCoins();
            }
        }
    }

    public static float GetHighScore()
    {
        return PlayerPrefs.GetFloat(HighScoreKey, 0f);
    }

    public static void TrySetHighScore(float score)
    {
        float current = GetHighScore();
        if (score > current)
        {
            PlayerPrefs.SetFloat(HighScoreKey, score);
            PlayerPrefs.Save();
        }
    }

    public static void ResetData()
    {
        PlayerPrefs.DeleteKey(CoinsKey);
        PlayerPrefs.DeleteKey(MoveSpeedKey);
        PlayerPrefs.DeleteKey(HealthKey);
        PlayerPrefs.DeleteKey(PickupKey);
        PlayerPrefs.DeleteKey(MaxWeaponsKey);
        PlayerPrefs.DeleteKey(HighScoreKey);
        PlayerPrefs.Save();

        // Update in-memory values if available
        if (CoinController.instance != null)
        {
            CoinController.instance.currentCoins = 0;
            if (UIController.instance != null)
                UIController.instance.UpdateCoins();
        }

        if (PlayerStatController.instance != null)
        {
            PlayerStatController.instance.moveSpeedLevel = 0;
            PlayerStatController.instance.healthLevel = 0;
            PlayerStatController.instance.pickupRangeLevel = 0;
            PlayerStatController.instance.maxWeaponsLevel = 0;
        }

        if (UIController.instance != null)
        {
            UIController.instance.UpdateHighScoreDisplay();
        }
    }
}
