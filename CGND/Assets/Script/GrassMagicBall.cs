using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GrassMagicBall : MonoBehaviour
{
    [SerializeField] private GameObject grassballDestroy;

    private void Start() {
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Ground")) {
            GrassTilemapManager.instance.StartSpread(transform.position);
            DestroyBall();
        }

        if (other.gameObject.CompareTag("Enemy")) {
            // MagicPoint.Instance.IncreaseMP();
            DestroyBall();
        }
    }

    private void DestroyBall() {
        Instantiate(grassballDestroy, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
