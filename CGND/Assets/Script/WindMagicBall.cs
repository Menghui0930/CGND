using System.Collections;
using UnityEngine;

public class WindMagicBall : MonoBehaviour {
    public static bool onHitBoostEnabled = false;

    [SerializeField] private GameObject windBallDestroy;
    [SerializeField] private float speedBonus = 1f;
    [SerializeField] private float boostDuration = 0.5f;
    [SerializeField] private int hurt_Damage;
    public string element;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
            if (onHitBoostEnabled)
                ApplySpeedBoost();
            // 先检查有没有盾
            ShieldController shield = other.GetComponentInParent<ShieldController>();
            if (shield != null && shield.gameObject.activeSelf) {
                // 有盾 → 攻击盾
                ShieldElement attackElement = ElementStringToEnum(element);
                shield.TakeHit(attackElement);
            } else {
                // 没盾 → 攻击本体
                EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
                enemy?.TakeDamage(hurt_Damage);
            }

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

    private ShieldElement ElementStringToEnum(string elem) {
        return elem switch {
            "Water" => ShieldElement.Blue,
            "Grass" => ShieldElement.Green,
            "Wind" => ShieldElement.Yellow,
            _ => ShieldElement.Blue
        };
    }
}