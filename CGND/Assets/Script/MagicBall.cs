using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class MagicBall : MonoBehaviour
{
    [SerializeField] private GameObject waterballDestroy;
    [SerializeField] private int Damage;
    public string element;
    private bool hasHit = false; 

    private void OnTriggerEnter2D(Collider2D other) {
        if(hasHit) return;

        if (other.CompareTag("Ground") || other.CompareTag("Wall")) {
            hasHit = true;
            GameObject waterball = Instantiate(waterballDestroy,transform.position,Quaternion.identity);
            Destroy(gameObject);
        }

        
        if (other.gameObject.CompareTag("Enemy")) {
            hasHit = true;
            MagicPoint.Instance.IncreaseMP();
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null) {
                enemy.TakeDamage(10);
            }
            GameObject waterball = Instantiate(waterballDestroy, transform.position, Quaternion.identity);
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
}
