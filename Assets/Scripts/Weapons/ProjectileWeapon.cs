using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    public EnemyDamager damager;
    public Projectile projectile;

    private float shotCounter;

    public float weaponRange;
    public LayerMask whatIsEnemy;

    void Start()
    {
        SetStats();
    }

    void Update()
    {
        if (statsUpdated == true)
        {
            statsUpdated = false;
            SetStats();
        }

        shotCounter -= Time.deltaTime;

        if (shotCounter <= 0)
        {
            shotCounter = stats[weaponLevel].timeBetweenAttacks;

            Collider2D[] enemies = Physics2D.OverlapCircleAll(
                transform.position,
                weaponRange * stats[weaponLevel].range,
                whatIsEnemy
            );

            if (enemies.Length > 0)
            {
                // Get sorted enemies (nearest first)
                List<Transform> nearestEnemies = GetNearestEnemies(enemies);

                float amount = stats[weaponLevel].amount;

                for (int i = 0; i < amount; i++)
                {
                    // Loop back if fewer enemies than shots
                    Transform target = nearestEnemies[i % nearestEnemies.Count];

                    ShootAtTarget(target);
                }

                SFXManager.instance.PlaySFXPitched(6);
            }
        }
    }

    void ShootAtTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90f;

        // Optional spread (feel free to tweak or remove)
        float spread = Random.Range(-5f, 5f);

        Quaternion rotation = Quaternion.AngleAxis(angle + spread, Vector3.forward);

        Instantiate(projectile, transform.position, rotation)
            .gameObject.SetActive(true);
    }

    List<Transform> GetNearestEnemies(Collider2D[] enemies)
    {
        List<Transform> sortedEnemies = new List<Transform>();

        foreach (Collider2D enemy in enemies)
        {
            sortedEnemies.Add(enemy.transform);
        }

        sortedEnemies.Sort((a, b) =>
        {
            float distA = Vector2.Distance(transform.position, a.position);
            float distB = Vector2.Distance(transform.position, b.position);
            return distA.CompareTo(distB);
        });

        return sortedEnemies;
    }

    void SetStats()
    {
        damager.damageAmount = stats[weaponLevel].damage;
        damager.lifeTime = stats[weaponLevel].duration;

        damager.transform.localScale = Vector3.one * stats[weaponLevel].range;

        shotCounter = 0f;

        projectile.moveSpeed = stats[weaponLevel].speed;
    }
}