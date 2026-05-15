using UnityEngine;

public class TrackingBullet : MonoBehaviour
{
    public Transform target;
    public float speed = 6f;
    public int damage = 10;
    public float lifetime = 10f;       // destroy after 10s if nothing hit

    private float lifetimeTimer = 0f;

    void Update()
    {
        // count lifetime
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // follow player
        if (target == null) return;
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        // rotate to face direction
        Vector2 dir = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Boss")
            return;
        // hit player
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Tracking bullet hit player! Damage: " + damage);
           
            Destroy(gameObject);
        }

        // hit wall or ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Tracking bullet hit wall!");
            Destroy(gameObject);
        }
    }
}