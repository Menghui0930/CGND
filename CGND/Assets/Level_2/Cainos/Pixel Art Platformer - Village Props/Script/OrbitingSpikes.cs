using UnityEngine;

public class OrbitingSpikes : MonoBehaviour
{
    
    public float speed = 5f;
    public Transform[] points;

    private int i;

    void Start()
    {
        transform.position = points[0].position;

    }


    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.01f)
        {
            i++;
            if (i == points.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed = Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            Debug.Log("Player Hit");
           
        }
    }
}
