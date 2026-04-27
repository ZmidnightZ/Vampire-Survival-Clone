using UnityEngine;

public class WeaponThrower : Weapon
{
    public EnemyDamager damager;

    private float throwCounter;

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

        throwCounter -= Time.deltaTime;

        if (throwCounter <= 0)
        {
            throwCounter = stats[weaponLevel].timeBetweenAttacks;

            for (int i = 0; i < stats[weaponLevel].amount; i++)
            {
                // spawn at player position
                Vector3 spawnPos = transform.position;

                EnemyDamager newAxe = Instantiate(damager, spawnPos, Quaternion.identity);

                // simple axe throw (slight random left/right, always go up)
                float x = Random.Range(-10f, 10f);
                //float y = 8f;

                Rigidbody2D rb = newAxe.GetComponent<Rigidbody2D>();
                //rb.linearVelocity = new Vector2(x, y);

                newAxe.gameObject.SetActive(true);
            }

            SFXManager.instance.PlaySFXPitched(4);
        }
    }

    void SetStats()
    {
        damager.damageAmount = stats[weaponLevel].damage;
        damager.lifeTime = stats[weaponLevel].duration;
        damager.transform.localScale = Vector3.one * stats[weaponLevel].range;

        throwCounter = 0f;
    }
}