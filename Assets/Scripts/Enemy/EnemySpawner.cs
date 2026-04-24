using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform minSpawn, maxSpawn;

    public int maxEnemies = 200;
    public int checkPerFrame = 10;

    private Transform player;
    private float despawnDistance;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private int enemyCheckIndex;

    void Start()
    {
        player = PlayerHealthController.instance.transform;
        despawnDistance = Vector3.Distance(transform.position, maxSpawn.position) + 5f;
    }

    void Update()
    {
        transform.position = player.position;

        CleanupEnemies();
    }

    // MAIN SPAWN METHOD
    public GameObject SpawnEnemy(GameObject prefab)
    {
        if (prefab == null) return null;
        if (activeEnemies.Count >= maxEnemies) return null;

        Vector3 pos = SelectSpawnPoint();
        return SpawnAt(prefab, pos);
    }

    public GameObject SpawnAt(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return null;
        if (activeEnemies.Count >= maxEnemies) return null;

        GameObject enemy = AutoPool.Instance.Spawn(prefab, position, Quaternion.identity);

        if (enemy != null)
        {
            ApplyDifficulty(enemy);
            activeEnemies.Add(enemy);
        }

        return enemy;
    }

    public GameObject SpawnAndRegister(GameObject prefab, Vector3 position)
    {
        return SpawnAt(prefab, position);
    }

    // DIFFICULTY SCALING
    void ApplyDifficulty(GameObject enemy)
    {
        var ec = enemy.GetComponent<EnemyController>();
        if (ec != null)
        {
            float d = DifficultyManager.instance.difficulty;

            ec.health *= (1f + d * 2f);
            ec.damage *= (1f + d * 1.5f);
        }
    }

    // SPAWN POSITION 
    public Vector3 SelectSpawnPoint()
    {
        Vector3 spawnPoint = Vector3.zero;

        bool vertical = Random.value > 0.5f;

        if (vertical)
        {
            spawnPoint.y = Random.Range(minSpawn.position.y, maxSpawn.position.y);
            spawnPoint.x = Random.value > 0.5f ? maxSpawn.position.x : minSpawn.position.x;
        }
        else
        {
            spawnPoint.x = Random.Range(minSpawn.position.x, maxSpawn.position.x);
            spawnPoint.y = Random.value > 0.5f ? maxSpawn.position.y : minSpawn.position.y;
        }

        return spawnPoint;
    }

    // CLEANUP
    void CleanupEnemies()
    {
        int checks = 0;

        while (checks < checkPerFrame && activeEnemies.Count > 0)
        {
            if (enemyCheckIndex >= activeEnemies.Count)
                enemyCheckIndex = 0;

            var enemy = activeEnemies[enemyCheckIndex];

            if (enemy == null || !enemy.activeInHierarchy)
            {
                activeEnemies.RemoveAt(enemyCheckIndex);
            }
            else
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);

                if (dist > despawnDistance)
                {
                    var pooled = enemy.GetComponent<PooledObject>();
                    if (pooled != null) pooled.Despawn();
                    else Destroy(enemy);

                    activeEnemies.RemoveAt(enemyCheckIndex);
                }
                else
                {
                    enemyCheckIndex++;
                }
            }

            checks++;
        }
    }
}