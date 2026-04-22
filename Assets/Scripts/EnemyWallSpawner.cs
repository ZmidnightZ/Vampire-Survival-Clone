using System.Collections;
using UnityEngine;

public static class EnemyWallSpawner
{
    public static IEnumerator DoEnemyWall(EnemySpawner spawner, SpawnEvent e)
    {
        if (e == null || e.enemyPrefab == null) yield break;

        Vector2 dir = GetRandomDirection();

        Vector3 basePos = spawner != null ? spawner.SelectSpawnPoint() : Vector3.zero;
        float spacing = 1.2f;

        for (int i = 0; i < e.amount; i++)
        {
            Vector3 offset;
            if (Mathf.Abs(dir.x) > 0f)
                offset = new Vector3(0f, i * spacing, 0f);
            else
                offset = new Vector3(i * spacing, 0f, 0f);

            offset += (Vector3)(Random.insideUnitCircle * 0.2f);

            Vector3 spawnPos = basePos + offset;

            GameObject enemy = null;
            if (spawner != null)
            {
                enemy = spawner.SpawnAndRegister(e.enemyPrefab, spawnPos);
            }
            else
            {
                enemy = AutoPool.Instance.Spawn(e.enemyPrefab, spawnPos, Quaternion.identity);
            }

            if (enemy != null)
            {
                var ec = enemy.GetComponent<EnemyController>();
                if (ec != null) ec.SetDirection(dir);
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    static Vector2 GetRandomDirection()
    {
        int dir = Random.Range(0, 4);
        switch (dir)
        {
            case 0: return Vector2.left;
            case 1: return Vector2.right;
            case 2: return Vector2.up;
            default: return Vector2.down;
        }
    }
}
