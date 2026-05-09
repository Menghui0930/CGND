using System.Collections;
using UnityEngine;

public class WindMagicBall : MonoBehaviour {
    public static bool onHitBoostEnabled = false;

    [SerializeField] private GameObject windBallDestroy;
    [SerializeField] private float speedBonus = 1f;
    [SerializeField] private float boostDuration = 0.5f;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            if (onHitBoostEnabled)
                ApplySpeedBoost();
            DestroyBall();
        }
        if (other.CompareTag("Ground")) {
            DestroyBall();
        }
    }

    private void ApplySpeedBoost() {
        PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
        if (movement != null)
            movement.StartCoroutine(BoostCoroutine(movement));
    }

    private IEnumerator BoostCoroutine(PlayerMovement movement) {
        movement.AddSpeedBonus(speedBonus);
        yield return new WaitForSeconds(boostDuration);
        movement.AddSpeedBonus(-speedBonus);
    }

    private void DestroyBall() {
        Instantiate(windBallDestroy, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}