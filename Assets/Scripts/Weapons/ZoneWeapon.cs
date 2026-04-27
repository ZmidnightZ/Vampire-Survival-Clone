using UnityEngine;

public class ZoneWeapon : Weapon
{
    public EnemyDamager damager;

    private float spawnTime, spawnCounter;

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

        spawnCounter -= Time.deltaTime;

        if (spawnCounter <= 0f)
        {
            spawnCounter = spawnTime;

            EnemyDamager newZone = Instantiate(damager, transform.position, Quaternion.identity, transform);

            newZone.damageAmount = stats[weaponLevel].damage;
            newZone.lifeTime = stats[weaponLevel].duration;
            newZone.timeBetweenDamage = stats[weaponLevel].speed;
            newZone.transform.localScale = Vector3.one * stats[weaponLevel].range;

            newZone.gameObject.SetActive(true);

            SFXManager.instance.PlaySFXPitched(10);
        }
    }

    void SetStats()
    {
        spawnTime = stats[weaponLevel].timeBetweenAttacks;
        spawnCounter = 0f;
    }
}