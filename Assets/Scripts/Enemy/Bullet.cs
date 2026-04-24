using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 3f;

    public int damageAmount;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           DamagePlayer(damageAmount);
            Destroy(gameObject);
        }
    }

    void DamagePlayer(int damageAmount)
    {
        PlayerHealthController playerHealth = PlayerHealthController.instance;
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }
}