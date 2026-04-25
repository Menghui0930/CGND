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
    }
}
