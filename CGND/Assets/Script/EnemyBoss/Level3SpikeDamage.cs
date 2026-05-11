using UnityEngine;

public class Level3SpikeDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Player")) {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null) {
                playerHealth.LoseLife();
            }
        }
    }
}
