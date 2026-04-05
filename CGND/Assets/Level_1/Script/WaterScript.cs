using UnityEngine;

public class WaterScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerMotor playerMotor = collision.gameObject.GetComponentInParent<PlayerMotor>();
            if (playerMotor != null)
            {
                Health.OnDeath?.Invoke(playerMotor);
            }
        }
    }
}
