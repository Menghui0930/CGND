using UnityEngine;

public class JumpMushroom : MonoBehaviour
{
    private float bounce = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce);
        }
            

        

    }
}
