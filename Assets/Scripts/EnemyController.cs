using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody2D theRB;
    public float moveSpeed;
    public bool isElite = false;
    private float originalMoveSpeed;
    private float originalHealth;
    private Vector3 originalScale;
    private bool originalStored = false;
    private Vector2 forcedDirection;
    private bool useForcedDirection = false;
    private Transform target;

    public float damage;

    public float hitWaitTime = 1f;
    private float hitCounter;

    public float health = 5f;

    public float knockBackTime = .5f;
    private float knockBackCounter;

    public int expToGive = 1;

    public int coinValue = 1;
    public float coinDropRate = .5f;
    public HealthPickup healthPickup;
    // 1% drop rate for the healing item
    public float healthDropRate = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        //target = FindObjectOfType<PlayerController>().transform;
        target = PlayerHealthController.instance.transform;
    }

    void OnEnable()
    {
        // store original stats once per instance
        if (!originalStored)
        {
            originalMoveSpeed = moveSpeed;
            originalHealth = health;
            originalScale = transform.localScale;
            originalStored = true;
        }

        if (isElite)
        {
            moveSpeed = originalMoveSpeed * 1.5f;
            health = originalHealth * 3f;
            transform.localScale = originalScale * 1.3f;
        }
        else
        {
            // ensure non-elite uses original values
            moveSpeed = originalMoveSpeed;
            health = originalHealth;
            transform.localScale = originalScale;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.instance.gameObject.activeSelf == true)
        {
            if (knockBackCounter > 0)
            {
                knockBackCounter -= Time.deltaTime;

                if (moveSpeed > 0)
                {
                    moveSpeed = -moveSpeed * 2f;
                }

                if (knockBackCounter <= 0)
                {
                    moveSpeed = Mathf.Abs(moveSpeed * .5f);
                }
            }

            if (useForcedDirection)
            {
                theRB.linearVelocity = forcedDirection * moveSpeed;
            }
            else
            {
                theRB.linearVelocity = (target.position - transform.position).normalized * moveSpeed;
            }

            if (hitCounter > 0f)
            {
                hitCounter -= Time.deltaTime;
            }
        } else
        {
            theRB.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" && hitCounter <= 0f)
        {
            PlayerHealthController.instance.TakeDamage(damage);

            hitCounter = hitWaitTime;
        }
    }

    public void TakeDamage(float damageToTake)
    {
        health -= damageToTake;

        if(health <= 0)
        {
            Destroy(gameObject);

            ExperienceLevelController.instance.SpawnExp(transform.position, expToGive);

            if(Random.value <= coinDropRate)
            {
                CoinController.instance.DropCoin(transform.position, coinValue);
            }

            if (healthPickup != null && Random.value <= healthDropRate)
            {
                Instantiate(healthPickup, transform.position + new Vector3(.2f, .1f, 0f), Quaternion.identity);
            }

            SFXManager.instance.PlaySFXPitched(0);
        } else
        {
            SFXManager.instance.PlaySFXPitched(1);
        }

        DamageNumberController.instance.SpawnDamage(damageToTake, transform.position);
    }

    public void TakeDamage(float damageToTake, bool shouldKnockback)
    {
        TakeDamage(damageToTake);

        if(shouldKnockback == true)
        {
            knockBackCounter = knockBackTime;
        }
    }

    // Force this enemy to move in a specific direction (useful for wall/wave spawns)
    public void SetDirection(Vector2 dir)
    {
        forcedDirection = dir.normalized;
        useForcedDirection = true;
    }
}
