using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float speed = 5f;
    public Vector2 direction;

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = direction * speed;
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject
                .GetComponentInParent<Health>();
            if (playerHealth != null)
            {
                playerHealth.LoseLife();
            }
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);

            if (collision.gameObject.CompareTag("Boss"))
            {
                BossHealth bossHealth = collision.gameObject
                    .GetComponent<BossHealth>();
                if (bossHealth != null)
                {
                    bossHealth.TakeDamage(1);
                }
                Destroy(gameObject);
            }
        }
    }
}