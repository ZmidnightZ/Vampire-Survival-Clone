using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnEventType
{
    EnemyWall,
    ChargeCircle,
}

[System.Serializable]
public class SpawnEvent
{
    public string eventName;
    public SpawnEventType eventType;

    public GameObject enemyPrefab;

    public int amount = 10;
    public float duration = 3f;

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
            case SpawnEventType.EnemyWall:
                // forward to the dedicated wall spawner helper
                StartCoroutine(EnemyWallSpawner.DoEnemyWall(spawner, selected));
                break;
            case SpawnEventType.ChargeCircle:
                StartCoroutine(DoChargeCircle(selected));
                break;
            // Rush and EliteSpawn event types removed
        }
    }

    IEnumerator DoChargeCircle(SpawnEvent e)
    {
        // Replaced circular charge with spawning a small group of 5 enemies.
        if (spawner == null || e == null || e.enemyPrefab == null) yield break;

        int groupCount = 5;
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

    SpawnEvent GetWeightedRandomEvent()
    {
        float totalWeight = 0f;
        foreach (var e in events)
            totalWeight += e.weight;

        float randomPoint = Random.Range(0f, totalWeight);
        foreach (var e in events)
        {
            if (randomPoint < e.weight) return e;
            randomPoint -= e.weight;
        }

        return events.Count > 0 ? events[0] : null;
    }

}
