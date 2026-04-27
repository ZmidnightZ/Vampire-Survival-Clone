using UnityEngine;

public class ZoneWeapon : Weapon
{
    public EnemyDamager damager;

    private float spawnCounter;
    private float spawnDelay = 0.5f;

    private EnemyDamager currentZone;

    void Start()
    {
        SetStats();
    }

    void Update()
    {
        if (statsUpdated)
        {
            statsUpdated = false;
            SetStats();
        }

        // If zone still exists → do nothing
        if (currentZone != null)
        {
            return;
        }

        // Wait for blink delay
        spawnCounter -= Time.deltaTime;

        if (spawnCounter <= 0f)
        {
            SpawnZone();
        }
    }

    void SpawnZone()
    {
        currentZone = Instantiate(damager, transform.position, Quaternion.identity, transform);

        currentZone.damageAmount = stats[weaponLevel].damage;
        currentZone.lifeTime = stats[weaponLevel].duration;
        currentZone.timeBetweenDamage = stats[weaponLevel].speed;
        currentZone.transform.localScale = Vector3.one * stats[weaponLevel].range;

        currentZone.gameObject.SetActive(true);

        spawnCounter = spawnDelay;

        SFXManager.instance.PlaySFXPitched(10);
    }

    void SetStats()
    {
        spawnCounter = 0f;
    }
}