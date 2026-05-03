using UnityEngine;

public class WaterScript : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !triggered)
        {
            triggered = true;

            // 扣一条命
            Health playerHealth = collision.gameObject.GetComponentInParent<Health>();
            if (playerHealth != null)
            {
                playerHealth.LoseLife();
            }

            // 强制触发 Respawn
            PlayerMotor playerMotor = collision.gameObject.GetComponentInParent<PlayerMotor>();
            if (playerMotor != null)
            {
                Health.OnDeath?.Invoke(playerMotor);
            }

            Invoke("ResetTrigger", 2f);
        }
    }

    private void ResetTrigger()
    {
        triggered = false;
    }
}