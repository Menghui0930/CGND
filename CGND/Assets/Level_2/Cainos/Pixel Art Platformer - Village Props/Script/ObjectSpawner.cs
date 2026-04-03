using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform spawnPoint; // optional: where to spawn

    public void SpawnObject()
    {
        Vector3 position = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Instantiate(prefabToSpawn, position, Quaternion.identity);
    }
}