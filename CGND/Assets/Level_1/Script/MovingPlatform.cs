using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform[] points;
    public float MoveSpeed;
    public int currrentPoint;

    public Transform platform;

    void Start()
    {
        platform.position = points[currrentPoint].position;
    }

    void Update()
    {
        platform.position = Vector3.MoveTowards(platform.position, points[currrentPoint].position, MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(platform.position, points[currrentPoint].position) < 0.5f) {
            currrentPoint++;

            if (currrentPoint >= points.Length) {
                currrentPoint = 0;
            }
        }
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
