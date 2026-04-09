using UnityEngine;

public class WaterMagicBall : MonoBehaviour
{
    [SerializeField] private GameObject waterballDestroy;

    [SerializeField] private int maxBounces = 2;
    private int bounces = 0;
    private Rigidbody2D theRB;

    private void Start() {
        theRB = GetComponent<Rigidbody2D>();
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Ground") || other.CompareTag("Wall")) {
            if (bounces >= maxBounces) {
                DestroyBall();
                return;
            }

            Vector2 velocity = theRB.linearVelocity;
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                velocity.normalized,
                0.8f,
                LayerMask.GetMask("Ground", "Wall")
            );

            if (hit.collider != null) {
                // Base on hit normal reflect the velocity
                Debug.Log("flip");
                Vector2 reflected = Vector2.Reflect(velocity, hit.normal);
                theRB.linearVelocity = reflected;
            } else {
                // When it bounce at ceiling, raycast will a little bit hard 
                theRB.linearVelocity = new Vector2(velocity.x, -velocity.y);
            }

            bounces++;
        }

        /*
        if (other.CompareTag("Enemy")) {
            // Damage
            DestroyBall();
        }
        */
    }

    private void DestroyBall() {
        Instantiate(waterballDestroy, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
