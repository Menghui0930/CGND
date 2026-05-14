using UnityEngine;

public class Tornado : MonoBehaviour
{
    [SerializeField] private GameObject waterballDestroy;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Wall")) {
            GameObject waterball = Instantiate(waterballDestroy, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (other.CompareTag("Grass")) {
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Enemy")) {
            MagicPoint.Instance.IncreaseMP();
            Destroy(gameObject);
        }

        if (other.CompareTag("Boss"))
        {
            Debug.Log("Attacking enemy!!!");
            BossController boss = other.GetComponentInParent<BossController>();
            boss?.TakeDamage(5f);
            MagicPoint.Instance.IncreaseMP();
            GameObject waterball = Instantiate(waterballDestroy, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
