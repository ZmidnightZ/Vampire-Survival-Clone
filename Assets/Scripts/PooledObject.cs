using UnityEngine;

public class PooledObject : MonoBehaviour
{
    public GameObject prefab;

    public void Despawn()
    {
        if (AutoPool.Instance != null)
        {
            AutoPool.Instance.Despawn(gameObject, prefab);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
