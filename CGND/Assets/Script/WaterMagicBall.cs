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
                Vector2 reflected = Vector2.Reflect(velocity, hit.normal);
                theRB.linearVelocity = reflected;
                //Debug.Log(theRB.linearVelocity);
                FlipBall(reflected); 
            } else {
                // Sometime raycast cannot detach ceiling so it will use this
                Vector2 flipped = new Vector2(velocity.x, -velocity.y);
                theRB.linearVelocity = flipped;
                FlipBall(flipped); 
            }

            bounces++;
        }

        if (other.gameObject.CompareTag("Enemy")) {
            //MagicPoint.Instance.IncreaseMP();
            DestroyBall();
        }
    }

    private void FlipBall(Vector2 velocity) {
        if (Mathf.Abs(velocity.x) < 0.01f) return;
        //Debug.Log("flip");
        float absX = Mathf.Abs(transform.localScale.x);
        float dirX = velocity.x > 0 ? -absX : absX;
        //Debug.Log(dirX);
        transform.localScale = new Vector3(dirX, transform.localScale.y, transform.localScale.z);
    }

    private void DestroyBall() {
        Instantiate(waterballDestroy, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
