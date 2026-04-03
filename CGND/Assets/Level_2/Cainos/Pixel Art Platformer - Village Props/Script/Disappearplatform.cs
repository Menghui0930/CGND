using UnityEngine;
using System.Collections;


public class Disappearplatform : MonoBehaviour
{
    public float fallDelay = 5f;
    public float respawnDelay = 3f;

    private SpriteRenderer spriteRenderer;
    private Collider2D platformCollider;
    private bool triggered = false;
   
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !triggered) 
        {
            triggered = true; 
            StartCoroutine(HandlePlatformCycle());
        }
    }

    private IEnumerator HandlePlatformCycle()
    {
        yield return new WaitForSeconds(fallDelay);

        spriteRenderer.enabled = false;
        platformCollider.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        spriteRenderer.enabled = true;
        platformCollider.enabled = true;
        triggered = false; 
    }
}

   
    

