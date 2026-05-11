using UnityEngine;

public class WaterMagicBall : MonoBehaviour
{
    [SerializeField] private GameObject waterballDestroy;

    [SerializeField] private int maxBounces = 2;
    [SerializeField] private int hurt_Damage;
    public string element;

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
            // 先检查有没有盾
            ShieldController shield = other.GetComponentInParent<ShieldController>();
            if (shield != null && shield.gameObject.activeSelf) {
                // 有盾 → 攻击盾
                ShieldElement attackElement = ElementStringToEnum(element);
                shield.TakeHit(attackElement);
            } else {
                // 没盾 → 攻击本体
                EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
                enemy?.TakeDamage(hurt_Damage);
            }

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

    private ShieldElement ElementStringToEnum(string elem) {
        return elem switch {
            "Water" => ShieldElement.Blue,
            "Grass" => ShieldElement.Green,
            "Wind" => ShieldElement.Yellow,
            _ => ShieldElement.Blue
        };
    }
}
