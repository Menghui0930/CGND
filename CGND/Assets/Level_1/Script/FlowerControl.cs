using UnityEngine;

public class FlowerControl : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D platformCollider;
    private bool isOpen = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        platformCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name.Contains("WaterBall") || other.CompareTag("WaterBall"))
        {
            OpenFlower();
            Destroy(other.gameObject);
        }
    }

    public void OpenFlower()
    {
        if (isOpen) return;
        isOpen = true;
        anim.SetTrigger("Open");

        Invoke("EnableSolidPlatform", 0.2f);
    }

    void EnableSolidPlatform()
    {

        platformCollider.isTrigger = false;
    }
}