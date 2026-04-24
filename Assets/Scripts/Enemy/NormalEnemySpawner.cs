using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemySpawner : MonoBehaviour
{
    public EnemySpawner spawner;

    [Header("Waves")]
    public List<SpawnWave> waves;

    private int currentWaveIndex = 0;

    private float waveTimer;
    private float spawnTimer;

    void Start()
    {
        if (spawner == null)
            spawner = Object.FindFirstObjectByType<EnemySpawner>();

        PreloadEnemies();

        StartWave(0);
    }

    void Update()
    {
        if (waves == null || waves.Count == 0) return;

        SpawnWave currentWave = waves[currentWaveIndex];

        // Wave timer
        waveTimer -= Time.deltaTime;
        if (waveTimer <= 0f)
        {
            NextWave();
            return;
        }

        // Spawn timer
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemies(currentWave);
            spawnTimer = GetScaledSpawnDelay(currentWave.spawnDelay);
        }
    }

    void StartWave(int index)
    {
        currentWaveIndex = Mathf.Clamp(index, 0, waves.Count - 1);

        SpawnWave wave = waves[currentWaveIndex];

        waveTimer = wave.duration;
        spawnTimer = 0f;

        Debug.Log("Start Wave: " + wave.waveName);
    }

    void NextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Count)
        {
            currentWaveIndex = waves.Count - 1;

            // increase difficulty instead
            DifficultyManager.instance.difficulty += 0.2f;
        }

        StartWave(currentWaveIndex);
    }

    void SpawnEnemies(SpawnWave wave)
    {
        if (wave.enemies == null || wave.enemies.Count == 0) return;

        float d = DifficultyManager.instance.difficulty;

        int amount = wave.baseAmount + Mathf.FloorToInt(d * 3);

        for (int i = 0; i < amount; i++)
        {
            GameObject prefab = wave.enemies[Random.Range(0, wave.enemies.Count)];

            Vector3 pos = spawner.SelectSpawnPoint();

            spawner.SpawnAndRegister(prefab, pos);
        }
    }

    float GetScaledSpawnDelay(float baseDelay)
    {
        float d = DifficultyManager.instance.difficulty;
        return baseDelay * Mathf.Lerp(1f, 0.5f, d);
    }

    void PreloadEnemies()
    {
        HashSet<GameObject> seen = new HashSet<GameObject>();

        foreach (var wave in waves)
        {
            foreach (var e in wave.enemies)
            {
                if (e != null && seen.Add(e))
                {
                    AutoPool.Instance.Preload(e, 50);
                }
            }
        }
    }
}

[System.Serializable]
public class SpawnWave
{
    public string waveName;

    public List<GameObject> enemies;

    public float duration = 15f;
    public float spawnDelay = 1.5f;

    public int baseAmount = 3;
}