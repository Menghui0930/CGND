using UnityEngine;

public class TriggerRespawn : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            other.GetComponentInParent<Health>().LoseLife();
            StartCoroutine(LevelManager.Instance.RespawnCo(true));
        }
    }
}
