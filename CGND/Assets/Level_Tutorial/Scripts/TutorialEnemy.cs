using UnityEngine;

public class TutorialEnemy : MonoBehaviour {
    private Animator anim;

    void Start() {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("MagicBall") || collision.CompareTag("WaterBall")) {
            anim.SetTrigger("Hit");
        }
    }
}