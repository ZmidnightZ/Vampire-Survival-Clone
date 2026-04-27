using UnityEngine;
using System.Collections.Generic;

public class SpinWeapon : Weapon
{
    public float rotateSpeed;

    public Transform holder, fireballToSpawn;

    public float timeBetweenSpawn;
    private float spawnCounter;

    public EnemyDamager damager;

    private List<Transform> activeFireballs = new List<Transform>();

    private bool isActive = false;   

    void Start()
    {
        SetStats();
    }

    void Update()
    {
        holder.rotation = Quaternion.Euler(0f,0f,holder.rotation.eulerAngles.z +(rotateSpeed * Time.deltaTime * stats[weaponLevel].speed));

        if (isActive)
        {
            if (holder.childCount == 0)
            {
                isActive = false;
                spawnCounter = timeBetweenSpawn; 
            }
        }
        else
        {
            spawnCounter -= Time.deltaTime;

            if (spawnCounter <= 0)
            {
                spawnCounter = timeBetweenSpawn;

                for (int i = 0; i < stats[weaponLevel].amount; i++)
                {
                    float rot = (360f / stats[weaponLevel].amount) * i;

                    Instantiate(fireballToSpawn,holder.position,Quaternion.Euler(0f, 0f, rot),holder).gameObject.SetActive(true);

                    SFXManager.instance.PlaySFX(8);
                }

                isActive = true;
            }
        }

        if (statsUpdated)
        {
            statsUpdated = false;
            SetStats();
        }
    }

    public void SetStats()
    {
        damager.damageAmount = stats[weaponLevel].damage;

        transform.localScale = Vector3.one * stats[weaponLevel].range;

        timeBetweenSpawn = stats[weaponLevel].timeBetweenAttacks;

        damager.lifeTime = stats[weaponLevel].duration;

        spawnCounter = Mathf.Min(spawnCounter, timeBetweenSpawn);
    }
}
