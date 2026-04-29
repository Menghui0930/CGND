using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [Header("Pool Settings")]
    public GameObject bulletPrefab;
    public int poolSize = 300;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject b = Instantiate(bulletPrefab, transform);
            b.SetActive(false);
            pool.Enqueue(b);
        }
    }

    public GameObject Get(Vector2 position, Quaternion rotation)
    {
        GameObject b = pool.Count > 0 ? pool.Dequeue() : Instantiate(bulletPrefab, transform);
        b.transform.position = position;
        b.transform.rotation = rotation;
        b.SetActive(true);
        return b;
    }

    public void Return(GameObject b)
    {
        b.SetActive(false);
        pool.Enqueue(b);
    }
}