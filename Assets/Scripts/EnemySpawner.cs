using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyToSpawn;

    // Timed enemy-type cycling
    public List<GameObject> enemyTypes;
    public float typeDuration = 60f; // seconds per type
    private float typeTimer = 0f;
    private int currentEnemyTypeIndex = 0;
    private int typesSeen = 0; // counts how many types have been cycled

    // difficulty multipliers applied after each cycle of 6 types
    public float healthMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float multiplierIncrease = 1.2f; // multiply health/damage by this after each 6 types

    public float timeToSpawn;
    private float spawnCounter;

    public Transform minSpawn, maxSpawn;

    private Transform target;

    private float despawnDistance;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public int checkPerFrame;
    private int enemyToCheck;

    public List<WaveInfo> waves;

    private int currentWave;

    private float waveCounter;

    // Start is called before the first frame update
    void Start()
    {
        // ensure pool exists and preload each enemy type
        if (waves != null)
        {
            var seen = new HashSet<GameObject>();
            foreach (var w in waves)
            {
                var prefab = w.enemyToSpawn;
                if (prefab != null && seen.Add(prefab))
                {
                    AutoPool.Instance.Preload(prefab, 50); // adjust 50 to expected simultaneous enemies of this type
                }

                // preload elites if present
                if (w.elitePrefab != null && seen.Add(w.elitePrefab))
                {
                    AutoPool.Instance.Preload(w.elitePrefab, 10);
                }
            }
        }

        if (enemyTypes != null)
        {
            var seenTypes = new HashSet<GameObject>();
            foreach (var e in enemyTypes)
            {
                if (e != null && seenTypes.Add(e))
                {
                    AutoPool.Instance.Preload(e, 20);
                }
            }
        }

        target = PlayerHealthController.instance.transform;

        despawnDistance = Vector3.Distance(transform.position, maxSpawn.position) + 4f;

        currentWave = -1;
        GoToNextWave();
    }

    // Update is called once per frame
    void Update()
    {
        // handle timed enemy-type cycling
        if (enemyTypes != null && enemyTypes.Count > 0)
        {
            typeTimer += Time.deltaTime;
            if (typeTimer >= typeDuration)
            {
                typeTimer -= typeDuration;
                currentEnemyTypeIndex++;
                typesSeen++;

                if (currentEnemyTypeIndex >= enemyTypes.Count)
                {
                    currentEnemyTypeIndex = 0;
                }

                // after every 6 types, increase difficulty
                if (typesSeen >= 6)
                {
                    typesSeen = 0;
                    healthMultiplier *= multiplierIncrease;
                    damageMultiplier *= multiplierIncrease;
                    Debug.Log("Difficulty increased: health x" + healthMultiplier + " dmg x" + damageMultiplier);
                }
            }
        }

        if(PlayerHealthController.instance.gameObject.activeSelf)
        {
            if (currentWave < waves.Count)
            {
                waveCounter -= Time.deltaTime;
                if(waveCounter <= 0)
                {
                    GoToNextWave();
                }

                spawnCounter -= Time.deltaTime;
                if(spawnCounter <= 0)
                {
                    spawnCounter = waves[currentWave].timeBetweenSpawns;

                    GameObject prefabToSpawn = null;

                    // if enemy types list is configured, use the current timed type
                    if (enemyTypes != null && enemyTypes.Count > 0)
                    {
                        prefabToSpawn = enemyTypes[currentEnemyTypeIndex];
                    }

                    // fallback to wave's enemy (with elite roll) if no timed types configured
                    if (prefabToSpawn == null)
                    {
                        prefabToSpawn = waves[currentWave].enemyToSpawn;
                        if (waves[currentWave].elitePrefab != null && Random.value < waves[currentWave].eliteChance)
                        {
                            prefabToSpawn = waves[currentWave].elitePrefab;
                        }
                    }

                    GameObject newEnemy = AutoPool.Instance.Spawn(prefabToSpawn, SelectSpawnPoint(), Quaternion.identity);

                    // Apply difficulty multipliers to spawned enemy (if it has EnemyController)
                    if (newEnemy != null)
                    {
                        var ec = newEnemy.GetComponent<EnemyController>();
                        if (ec != null)
                        {
                            ec.health *= healthMultiplier;
                            ec.damage *= damageMultiplier;
                        }
                        spawnedEnemies.Add(newEnemy);
                    }
                }
            }
        }

        transform.position = target.position;

        int checkTarget = enemyToCheck + checkPerFrame;

        while(enemyToCheck < checkTarget)
        {
            if(enemyToCheck < spawnedEnemies.Count)
            {
                if (spawnedEnemies[enemyToCheck] != null)
                {
                    if(Vector3.Distance(transform.position, spawnedEnemies[enemyToCheck].transform.position) > despawnDistance)
                    {
                        var obj = spawnedEnemies[enemyToCheck];
                        var pooled = obj.GetComponent<PooledObject>();
                        if(pooled != null)
                        {
                            pooled.Despawn();
                        }
                        else
                        {
                            Destroy(obj);
                        }

                        spawnedEnemies.RemoveAt(enemyToCheck);
                        checkTarget--;
                    }
                    else
                    {
                        enemyToCheck++;
                    }
                } else
                {
                    spawnedEnemies.RemoveAt(enemyToCheck);
                    checkTarget--;
                }
            } else
            {
                enemyToCheck = 0;
                checkTarget = 0;
            }
        }
    }

    public Vector3 SelectSpawnPoint()
    {
        Vector3 spawnPoint = Vector3.zero;

        bool spawnVerticalEdge = Random.Range(0f, 1f) > .5f;

        if(spawnVerticalEdge)
        {
            spawnPoint.y = Random.Range(minSpawn.position.y, maxSpawn.position.y);

            if(Random.Range(0f, 1f) > .5f)
            {
                spawnPoint.x = maxSpawn.position.x;
            } else
            {
                spawnPoint.x = minSpawn.position.x;
            }
        } else
        {
            spawnPoint.x = Random.Range(minSpawn.position.x, maxSpawn.position.x);

            if (Random.Range(0f, 1f) > .5f)
            {
                spawnPoint.y = maxSpawn.position.y;
            }
            else
            {
                spawnPoint.y = minSpawn.position.y;
            }
        }



        return spawnPoint;
    }

    public void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null) return;

        GameObject newEnemy = AutoPool.Instance.Spawn(prefab, SelectSpawnPoint(), Quaternion.identity);

        if (newEnemy != null)
        {
            var ec = newEnemy.GetComponent<EnemyController>();
            if (ec != null)
            {
                ec.health *= healthMultiplier;
                ec.damage *= damageMultiplier;
            }

            spawnedEnemies.Add(newEnemy);
        }
    }

    // Spawn at specific position and register. Returns spawned GameObject.
    public GameObject SpawnAndRegister(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return null;

        GameObject newEnemy = AutoPool.Instance.Spawn(prefab, position, Quaternion.identity);

        if (newEnemy != null)
        {
            var ec = newEnemy.GetComponent<EnemyController>();
            if (ec != null)
            {
                ec.health *= healthMultiplier;
                ec.damage *= damageMultiplier;
            }

            spawnedEnemies.Add(newEnemy);
        }

        return newEnemy;
    }


    public void GoToNextWave()
    {
        currentWave++;

        if(currentWave >= waves.Count)
        {
            currentWave = waves.Count - 1;
        }

        waveCounter = waves[currentWave].waveLength;
        spawnCounter = waves[currentWave].timeBetweenSpawns;
    }


}

[System.Serializable]
public class WaveInfo
{
    public GameObject enemyToSpawn;

    [Header("Elite Settings")]
    public GameObject elitePrefab;
    [Range(0f, 1f)] public float eliteChance = 0.05f;

    public float waveLength = 10f;
    public float timeBetweenSpawns = 1f;
}
