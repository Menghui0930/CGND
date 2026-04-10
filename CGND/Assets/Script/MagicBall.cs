using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class MagicBall : MonoBehaviour
{
    [SerializeField] private GameObject waterballDestroy;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Ground") || other.CompareTag("Wall")) {
            GameObject waterball = Instantiate(waterballDestroy,transform.position,Quaternion.identity);
            Destroy(gameObject);
        }

        /*
        if (other.gameObject.CompareTag("Enemy")) {
            // get damage
        }
        */
    }
}
