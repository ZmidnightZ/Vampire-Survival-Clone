using System.Collections.Generic;
using UnityEngine;

public class AutoPool : MonoBehaviour
{
    public static AutoPool Instance;

    private Dictionary<GameObject, Queue<GameObject>> pool = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        Instance = this;
    }

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (AutoPool.Instance == null)
            new GameObject("AutoPool").AddComponent<AutoPool>();

        if (!pool.ContainsKey(prefab))
        {
            pool[prefab] = new Queue<GameObject>();
        }

        GameObject obj;

        if (pool[prefab].Count > 0)
        {
            obj = pool[prefab].Dequeue();
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab);
            var po = obj.AddComponent<PooledObject>();
            po.prefab = prefab;
        }

        obj.transform.position = pos;
        obj.transform.rotation = rot;

        return obj;
    }

    public void Despawn(GameObject obj, GameObject prefab)
    {
        if (!pool.ContainsKey(prefab))
        {
            pool[prefab] = new Queue<GameObject>();
        }

        obj.SetActive(false);
        pool[prefab].Enqueue(obj);
    }

    public void Preload(GameObject prefab, int count)
    {
        if (Instance == null)
        {
            var go = new GameObject("AutoPool");
            Instance = go.AddComponent<AutoPool>();
        }

        if (!pool.ContainsKey(prefab))
            pool[prefab] = new Queue<GameObject>();

        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab);
            var po = obj.GetComponent<PooledObject>() ?? obj.AddComponent<PooledObject>();
            po.prefab = prefab;
            obj.SetActive(false);
            pool[prefab].Enqueue(obj);
        }
    }

}