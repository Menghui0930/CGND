using UnityEngine;

public class SpikeDamage : MonoBehaviour {
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.tag == "Player") {
            Health health = collision.GetComponent<Health>();
            if (health != null) {
                health.LoseLife();
            }
        }
    }
}