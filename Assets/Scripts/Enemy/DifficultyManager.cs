using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager instance;

    public float gameTime;
    public float difficulty; // 0 → ∞

    public float timeToMax = 300f; // 5 minutes

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        difficulty = gameTime / timeToMax;
    }

    public float SpawnRateMultiplier()
    {
        return Mathf.Lerp(1f, 0.4f, difficulty);
    }

    public int EnemyCountBonus()
    {
        return Mathf.FloorToInt(difficulty * 10f);
    }

    public float EliteChance()
    {
        return Mathf.Lerp(0.05f, 0.3f, difficulty);
    }
}