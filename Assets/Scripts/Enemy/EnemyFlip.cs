using UnityEngine;

public class EnemyFlip : MonoBehaviour
{
    public Transform player;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        sr.flipX = player.position.x > transform.position.x;
    }
}
