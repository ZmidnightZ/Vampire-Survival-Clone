using UnityEngine;

public class CloseAttackWeapon : Weapon
{
    public EnemyDamager damager;

    private float attackCounter;

    // Store last direction player moved
    // 1 = right, -1 = left
    private float lastDirection = 1f;

    void Start()
    {
        SetStats();
    }

    void Update()
    {
        // If weapon level changed → update stats
        if (statsUpdated == true)
        {
            statsUpdated = false;
            SetStats();
        }

        float input = Input.GetAxisRaw("Horizontal");

        // If player presses left or right, update direction
        if (input > 0)
        {
            lastDirection = 1f; // facing right
        }
        else if (input < 0)
        {
            lastDirection = -1f; // facing left
        }

        attackCounter -= Time.deltaTime;

        if (attackCounter <= 0)
        {
            attackCounter = stats[weaponLevel].timeBetweenAttacks;

            Attack();

            // Play sound
            SFXManager.instance.PlaySFXPitched(9);
        }
    }

    void Attack()
    {
        // Decide main direction
        float angle;

        if (lastDirection == 1f)
            angle = 0f;      // right
        else
            angle = 180f;    // left

        // Attack main side
        SpawnSide(angle);

        // Unlock second side at higher level
        if (weaponLevel >= 5)
        {
            if (angle == 0f)
                SpawnSide(180f); // also attack left
            else
                SpawnSide(0f);   // also attack right
        }
    }

    void SpawnSide(float angle)
    {
        float amount = stats[weaponLevel].amount;

        float spacing = 0.5f;

        for (int i = 0; i < amount; i++)
        {
            // Base horizontal position
            Vector3 offset = new Vector3(i * spacing, 0f, 0f);

            // Flip if attacking left
            if (angle == 180f)
            {
                offset.x = -offset.x;
            }

            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            Instantiate(damager,transform.position + offset,rot,transform).gameObject.SetActive(true);
        }
    }

    void SetStats()
    {
        damager.damageAmount = stats[weaponLevel].damage;
        damager.lifeTime = stats[weaponLevel].duration;

        // Increase size = increase range
        damager.transform.localScale = Vector3.one * stats[weaponLevel].range;

        // Attack immediately after upgrade
        attackCounter = 0f;
    }
}