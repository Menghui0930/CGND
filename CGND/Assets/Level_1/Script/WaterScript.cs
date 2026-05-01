using UnityEngine;

public class WaterScript : MonoBehaviour
{

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !triggered)
        {
            triggered = true;

            Health playerHealth = collision.gameObject.GetComponentInParent<Health>();
            if (playerHealth != null)
            {
                playerHealth.LoseLife();
            }

            PlayerMotor playerMotor = collision.gameObject.GetComponentInParent<PlayerMotor>();
            if (playerMotor != null)
            {
                Health.OnDeath?.Invoke(playerMotor);
            }
            // 1秒后重置
            Invoke("ResetTrigger", 1f);
        }
    }

    private void ResetTrigger()
    {
        triggered = false;
    }
}
