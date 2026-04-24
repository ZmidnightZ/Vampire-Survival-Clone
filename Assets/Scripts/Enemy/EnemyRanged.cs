using UnityEngine;

public class EnemyRanged : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stopDistance = 6f;
    public float retreatDistance = 2f;
    public float buffer = 0.3f; // prevents jitter

    [Header("Shooting")]
    public float fireRate = 0.5f;
    private float fireCooldown;

    public GameObject bulletPrefab;
    public Transform firePoint;

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // randomize first shot so enemies don't sync
        fireCooldown = Random.Range(0.2f, fireRate);
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        HandleMovement(distance);
        HandleShooting(distance);
    }

    void HandleMovement(float distance)
    {
        Vector2 dir = (player.position - transform.position).normalized;

        // MOVE TOWARD
        if (distance > stopDistance + buffer)
        {
            transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);
        }
        // MOVE AWAY
        else if (distance < retreatDistance - buffer)
        {
            transform.position -= (Vector3)(dir * moveSpeed * Time.deltaTime);
        }
        // ELSE: stay still (good shooting range)
    }

    void HandleShooting(float distance)
    {
        // Only shoot in optimal range
        if (distance > stopDistance || distance < retreatDistance)
            return;

        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f)
        {
            fireCooldown = fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.SetDirection(dir);
        }
    }
}