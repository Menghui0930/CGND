using UnityEngine;

public class JumpMushroom : MonoBehaviour
{
    private float bounceForce = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, bounceForce);
            Debug.Log("Bounced from top");
        }
            

        

    }
}
