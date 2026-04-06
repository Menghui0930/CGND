using UnityEngine;

public class OrbitSpikeBall : MonoBehaviour
{
    public Transform platform;      
    public float radius = 2f;       
    public float speed = 90f;       
    
    public float yOffset;
    public float startAngle = 0f;

    private float angle = 0f;

    private void Start()
    {
        angle = startAngle;
    }
    void Update()
    {
        angle += speed * Time.deltaTime;

        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

        Vector3 center = platform.position + new Vector3(0f, yOffset, 0f);
        transform.position = center + new Vector3(x, y, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Hit");
            
            
        }
    }
}
