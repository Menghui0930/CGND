using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassMagicBall : MonoBehaviour
{
    [SerializeField] private GameObject grassballDestroy;
    [SerializeField] private int hurt_Damage;
    public string element;

    private void Start() {
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Ground")) {
            GrassTilemapManager.instance.StartSpread(transform.position);
            DestroyBall();
        }

        if (other.gameObject.CompareTag("Enemy")) {
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
            // MagicPoint.Instance.IncreaseMP();
            DestroyBall();
        }
    }

    private void DestroyBall() {
        Instantiate(grassballDestroy, transform.position, Quaternion.identity);
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
