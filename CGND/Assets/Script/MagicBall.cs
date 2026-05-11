using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class MagicBall : MonoBehaviour
{
    [SerializeField] private GameObject waterballDestroy;
    [SerializeField] private int hurt_Damage;
    public string element;
    private bool hasHit = false; 

    private void OnTriggerEnter2D(Collider2D other) {
        if (hasHit) return;

        if (other.CompareTag("Ground") || other.CompareTag("Wall")) {
            hasHit = true;
            GameObject waterball = Instantiate(waterballDestroy, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }


        if (other.gameObject.CompareTag("Enemy")) {
            Debug.Log("hit Boss");

            hasHit = true;
            MagicPoint.Instance.IncreaseMP();

            // 先检查有没有盾
            ShieldController shield = other.GetComponentInParent<ShieldController>();
            if (shield != null && shield.gameObject.activeSelf) {
                // 有盾 → 攻击盾
                ShieldElement attackElement = ElementStringToEnum(element);
                shield.TakeHit(attackElement);
            } else {
                Debug.Log("No dun");
                // 没盾 → 攻击本体
                EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
                enemy?.TakeDamage(hurt_Damage);
            }

            Instantiate(waterballDestroy, transform.position, Quaternion.identity);
            Destroy(gameObject);

        }

        if (other.gameObject.CompareTag("Vine")) {
            element = PlayerElementSwitch.Instance_playerElementSwitch.GetCurrentElement();
            if (element == "Water") {
                hasHit = true;
                other.GetComponent<VineScript>()?.Grow();
                GameObject waterball = Instantiate(waterballDestroy, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
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

