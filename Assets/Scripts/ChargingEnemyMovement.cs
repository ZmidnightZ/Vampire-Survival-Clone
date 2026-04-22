using System.Collections;
using UnityEngine;

public class ChargingEnemyMovement : MonoBehaviour
{
    public float windUpTime = 0.9f;
    public float chargeDuration = 1.2f;
    public float speed = 6f;
    // option: spawn just outside the camera view and charge across the player
    public bool spawnOffscreenAndCross = false;
    public float crossSpeedMultiplier = 3f;
    public float crossMargin = 1f;

    // runtime state
    private Vector2 chargeDirection = Vector2.zero;
    private Vector2 lockedDirection = Vector2.zero;
    private bool useLockedDirection = false;

    [HideInInspector] public bool isWindingUp = false;
    [HideInInspector] public bool isCharging = false;

    void OnEnable()
    {
        // reset state when spawned/respawned
        StopAllCoroutines();
        isWindingUp = false;
        isCharging = false;
        useLockedDirection = false;
        lockedDirection = Vector2.zero;
        chargeDirection = Vector2.zero;
    }

    void Start()
    {
        if (spawnOffscreenAndCross && PlayerHealthController.instance != null)
        {
            SetupOffscreenCross();
        }
    }

    void SetupOffscreenCross()
    {
        var player = PlayerHealthController.instance.transform;
        if (player == null) return;

        Camera cam = Camera.main;
        float halfWidth = 8f;
        float halfHeight = 5f;
        if (cam != null && cam.orthographic)
        {
            halfHeight = cam.orthographicSize;
            halfWidth = halfHeight * cam.aspect;
        }

        // choose horizontal spawn (left or right) for a clear cross-over
        bool spawnLeft = Random.value > 0.5f;

        Vector3 spawnPos = player.position;
        float spawnX = player.position.x + (spawnLeft ? -(halfWidth + crossMargin) : (halfWidth + crossMargin));
        float spawnY = player.position.y + Random.Range(-halfHeight * 0.6f, halfHeight * 0.6f);
        spawnPos = new Vector3(spawnX, spawnY, transform.position.z);

        // place enemy immediately outside view
        transform.position = spawnPos;

        // lock direction toward the opposite side through the player
        Vector2 throughPlayer = ((Vector2)player.position - (Vector2)spawnPos).normalized;
        // ensure we aim past the player to the other side
        lockedDirection = throughPlayer;
        useLockedDirection = true;

        // increase speed for a fast cross and compute duration to travel across screen
        speed *= crossSpeedMultiplier;

        float oppositeX = player.position.x + (spawnLeft ? (halfWidth + crossMargin) : -(halfWidth + crossMargin));
        float travelDistance = Mathf.Abs(spawnX - oppositeX);
        // avoid zero duration
        chargeDuration = Mathf.Max(0.2f, travelDistance / (speed));

        // start the wind-up + charge automatically
        StartChargeLocked();
    }

    void Update()
    {
        if (isCharging)
        {
            transform.position += (Vector3)chargeDirection * speed * Time.deltaTime;
        }
        else
        {
            // idle behavior: slowly face the player
            // (optional) keep facing roughly toward player if present
            if (!useLockedDirection && PlayerHealthController.instance != null)
            {
                Vector2 dir = ((Vector2)PlayerHealthController.instance.transform.position - (Vector2)transform.position).normalized;
                // small smoothing could be added if desired
                chargeDirection = dir;
            }
        }
    }

    public void SetLockedDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude <= 0f) return;
        lockedDirection = dir.normalized;
        useLockedDirection = true;
    }

    public void StartChargeLocked()
    {
        StopAllCoroutines();
        StartCoroutine(ChargeLockedRoutine());
    }

    IEnumerator ChargeLockedRoutine()
    {
        isWindingUp = true;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.red;

        yield return new WaitForSeconds(windUpTime);

        isWindingUp = false;
        isCharging = true;

        // lock the charge direction
        chargeDirection = useLockedDirection ? lockedDirection : chargeDirection;

        yield return new WaitForSeconds(chargeDuration);

        isCharging = false;

        if (sr != null) sr.color = Color.white;

        // remove this enemy after the charge finishes
        var pooled = GetComponent<PooledObject>();
        if (pooled != null)
        {
            pooled.Despawn();
            yield break;
        }
        else
        {
            Destroy(gameObject);
            yield break;
        }
    }

    // optional API to trigger a normal charge toward player/current direction
    public void StartChargeNormal()
    {
        StopAllCoroutines();
        StartCoroutine(ChargeNormalRoutine());
    }

    IEnumerator ChargeNormalRoutine()
    {
        isWindingUp = true;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.red;

        yield return new WaitForSeconds(windUpTime);

        isWindingUp = false;
        isCharging = true;

        // ensure direction at start of charge
        if (!useLockedDirection && PlayerHealthController.instance != null)
        {
            chargeDirection = ((Vector2)PlayerHealthController.instance.transform.position - (Vector2)transform.position).normalized;
        }

        yield return new WaitForSeconds(chargeDuration);

        isCharging = false;

        if (sr != null) sr.color = Color.white;

        // remove this enemy after the charge finishes
        var pooled2 = GetComponent<PooledObject>();
        if (pooled2 != null)
        {
            pooled2.Despawn();
            yield break;
        }
        else
        {
            Destroy(gameObject);
            yield break;
        }
    }
}
