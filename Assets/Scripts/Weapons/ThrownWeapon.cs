using UnityEngine;

public class ThrownWeapon : MonoBehaviour
{
    public float throwUpForce = 8f;
    public float throwSideForce = 2f;
    public float rotateSpeed = 300f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float x = Random.Range(-throwSideForce, throwSideForce);
        float y = throwUpForce;

        rb.linearVelocity = new Vector2(x, y);
    }

    void Update()
    {
        if (rb.linearVelocity.x > 0)
        {
            transform.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
        }
    }
}