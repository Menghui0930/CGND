using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveDistance = 3.5f; 
    public float speed = 1.7f;        

    private Vector3 startPos;       

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float movement = Mathf.PingPong(Time.time * speed, moveDistance);

        transform.position = startPos + new Vector3(0, -movement, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
