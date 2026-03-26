using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            LevelManager.Instance.SetSpawnPoint(transform.position);
        }
    }
}
