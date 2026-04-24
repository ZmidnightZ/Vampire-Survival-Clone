using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnEventType
{
    Charge,
    RangedSpawn,
    EliteGroup 
}

[System.Serializable]
public class SpawnEvent
{
    public string eventName;
    public SpawnEventType eventType;

    public GameObject enemyPrefab;
    public GameObject elitePrefab;

    public int amount = 10;

    public float weight = 1f; // for weighted random selection
}

public class EventScheduler : MonoBehaviour
{
    public List<SpawnEvent> events;

    public float minTimeBetweenEvents = 30f;
    public float maxTimeBetweenEvents = 60f;

    public EnemySpawner spawner;

    private float timer;

    void Start()
    {
        if (spawner == null)
            spawner = Object.FindFirstObjectByType<EnemySpawner>();

        HashSet<GameObject> seen = new HashSet<GameObject>();

        foreach (var e in events)
        {
            if (e.enemyPrefab != null && seen.Add(e.enemyPrefab))
            {
                AutoPool.Instance.Preload(e.enemyPrefab, 30);
            }

            if (e.elitePrefab != null && seen.Add(e.elitePrefab))
            {
                AutoPool.Instance.Preload(e.elitePrefab, 20);
            }
        }

        ScheduleNextEvent();
    }

    void Update()
    {
        if (events == null || events.Count == 0) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            TriggerRandomEvent();
            ScheduleNextEvent();
        }
    }

    void ScheduleNextEvent()
    {
        timer = Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents);
    }

    void TriggerRandomEvent()
    {
        var selected = GetWeightedRandomEvent();
        if (selected == null) return;

        switch (selected.eventType)
        {
            case SpawnEventType.Charge:
                StartCoroutine(DoCharge(selected));
                break;

            case SpawnEventType.RangedSpawn:
                StartCoroutine(DoRangedSpawn(selected));
                break;

            case SpawnEventType.EliteGroup:
                StartCoroutine(DoEliteGroup(selected));
                break;
        }
    }

    IEnumerator DoCharge(SpawnEvent e)
    {
        // Replaced circular charge with spawning a small group of 5 enemies.
        if (spawner == null || e == null || e.enemyPrefab == null) yield break;

        int groupCount = e.amount;
        Vector3 playerPos = PlayerHealthController.instance.transform.position;

        // pick a single spawn point (uses existing spawner logic so enemies spawn outside/around camera)
        Vector3 baseSpawn = spawner.SelectSpawnPoint();

        List<ChargingEnemyMovement> group = new List<ChargingEnemyMovement>();

        for (int i = 0; i < groupCount; i++)
        {
            // small cluster near the chosen spawn point
            Vector2 jitter = Random.insideUnitCircle * 1.2f;
            Vector3 spawnPos = baseSpawn + (Vector3)jitter;

            GameObject enemy = spawner.SpawnAndRegister(e.enemyPrefab, spawnPos);

            if (enemy != null)
            {
                var charger = enemy.GetComponent<ChargingEnemyMovement>();
                if (charger != null)
                {
                    // lock direction toward player
                    Vector2 dir = (playerPos - spawnPos).normalized;
                    charger.SetLockedDirection(dir);

                    // rotate to face movement direction for clarity
                    enemy.transform.right = dir;

                    group.Add(charger);
                }
            }
        }

        // short warning / wind-up before the group charges
        yield return new WaitForSeconds(1f);

        foreach (var c in group)
        {
            if (c != null)
            {
                c.StartChargeLocked();
            }
        }
    }

    IEnumerator DoRangedSpawn(SpawnEvent e)
    {
        if (spawner == null || e == null || e.enemyPrefab == null) yield break;

        int groupSize = e.amount;
        int spawned = 0;

        while (spawned < e.amount)
        {
            Vector3 baseSpawn = spawner.SelectSpawnPoint();

            for (int i = 0; i < groupSize && spawned < e.amount; i++)
            {
                // small formation instead of random mess
                Vector2 offset = Random.insideUnitCircle * 1.5f;
                Vector3 spawnPos = baseSpawn + (Vector3)offset;

                GameObject enemy = spawner.SpawnAndRegister(e.enemyPrefab, spawnPos);

                spawned++;
            }

            // delay between squads (feels like waves)
            yield return new WaitForSeconds(1.5f);
        }
    }

    IEnumerator DoEliteGroup(SpawnEvent e)
    {
        if (spawner == null || e == null) yield break;

        GameObject prefab = e.elitePrefab != null ? e.elitePrefab : e.enemyPrefab;
        if (prefab == null) yield break;

        int count = e.amount;

        Vector3 playerPos = PlayerHealthController.instance.transform.position;
        Vector3 baseSpawn = spawner.SelectSpawnPoint();

        List<GameObject> group = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * 2f;
            Vector3 spawnPos = baseSpawn + (Vector3)offset;

            GameObject enemy = spawner.SpawnAndRegister(prefab, spawnPos);

            if (enemy != null)
            {
                var ec = enemy.GetComponent<EnemyController>();
                if (ec != null)
                {
                    ec.health *= 2f;
                    ec.damage *= 2f;
                }

                group.Add(enemy);
            }
        }

        yield return new WaitForSeconds(0.5f);

        foreach (var enemy in group)
        {
            if (enemy == null) continue;

            var charger = enemy.GetComponent<ChargingEnemyMovement>();
            if (charger != null)
            {
                Vector2 dir = (playerPos - enemy.transform.position).normalized;
                charger.SetLockedDirection(dir);
                charger.StartChargeLocked();
            }
        }
    }

    SpawnEvent GetWeightedRandomEvent()
    {
        float totalWeight = 0f;

        // store scaled weights temporarily
        List<float> scaledWeights = new List<float>();

        float d = DifficultyManager.instance != null ? DifficultyManager.instance.difficulty : 0f;

        // calculate scaled weights
        foreach (var e in events)
        {
            float weight = e.weight;

            // apply difficulty scaling
            if (e.eventType == SpawnEventType.RangedSpawn)
                weight *= Mathf.Lerp(0.5f, 2f, d);

            if (e.eventType == SpawnEventType.Charge)
                weight *= Mathf.Lerp(1f, 1.5f, d);

            scaledWeights.Add(weight);
            totalWeight += weight;
        }

        // pick random
        float randomPoint = Random.Range(0f, totalWeight);

        // select event
        for (int i = 0; i < events.Count; i++)
        {
            if (randomPoint < scaledWeights[i])
                return events[i];

            randomPoint -= scaledWeights[i];
        }

        return events.Count > 0 ? events[0] : null;
    }
}
