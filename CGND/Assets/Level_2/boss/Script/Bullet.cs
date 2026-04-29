using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 8f;
    public float lifetime = 5f;

    private Vector2 direction;
    private float timer;

    public void Init(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
        timer = 0f;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        timer += Time.deltaTime;
        if (timer >= lifetime)
            BulletPool.Instance.Return(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            BulletPool.Instance.Return(gameObject);
        }
    }
}