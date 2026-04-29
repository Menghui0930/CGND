using UnityEngine;

public class FireBallDestroy : MonoBehaviour
{
    public GameObject FireEffect;

    private void Start() {
        Destroy(gameObject, 4f);
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {

            Instantiate(FireEffect,transform.position,Quaternion.identity);
            // Hurt
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null) {
                playerHealth.LoseLife();
            }

            Destroy(gameObject);
        }

        if (other.CompareTag("Ground")) {
            Vector3 newPosition = new Vector3 (transform.position.x, transform.position.y - 0.5f, transform.position.z);
            Instantiate(FireEffect,newPosition,Quaternion.identity);
            Destroy(gameObject);
        }
    }

}
