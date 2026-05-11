using UnityEngine;

public class TutorialEnemy : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<MagicBall>() != null)
        {
            anim.SetTrigger("Hit");
        }
    }
}