using UnityEngine;
using System.Collections;


public class Disappearplatform : MonoBehaviour
{
    [Header("Shake Settings")]
    [Tooltip("Shake Count")]
    public int shakeCount = 3;

    [Tooltip("Distance of Each Shake")]
    public float shakeAmount = 0.15f;

    [Tooltip("Shake Time")]
    public float shakeDuration = 0.12f;

    [Header("Hide / Show")]
    [Tooltip("Respawn Time")]
    public float respawnDelay = 3f;

    private BoxCollider2D boxCol;
    [SerializeField]private SpriteRenderer theSR01;
    [SerializeField]private SpriteRenderer theSR02;
    [SerializeField] private SpriteRenderer theSR03;
    [SerializeField] private SpriteRenderer theSR04;
    private Vector3 _originPos;

    private bool isTrigger = false;

    void Start() {
        boxCol = GetComponent<BoxCollider2D>();
        _originPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Player") && !isTrigger) {
            isTrigger = true;
            //Debug.Log("Shake");
            StartCoroutine(ShakeAndDisappear());
        }
    }

    private IEnumerator ShakeAndDisappear() {

        for (int i = 0; i < shakeCount; i++) {
            // shake right
            yield return MoveTo(_originPos + Vector3.right * shakeAmount, shakeDuration / 2f);
            // shake left
            yield return MoveTo(_originPos + Vector3.left * shakeAmount, shakeDuration / 2f);
            // middle
            yield return MoveTo(_originPos, shakeDuration / 4f);
        }

        // 1. back position
        transform.position = _originPos;

        // 2. Hide platform
        SetPlatformVisible(false);

        // 3. wait respawn time
        yield return new WaitForSeconds(respawnDelay);

        // 4. Shaow Platform
        SetPlatformVisible(true);

        // Reset
        isTrigger = false;
    }

    IEnumerator MoveTo(Vector3 target, float duration) {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        transform.position = target;
    }

    void SetPlatformVisible(bool visible) {
        theSR01.enabled = visible;
        theSR02.enabled = visible;
        theSR03.enabled = visible;
        theSR04.enabled = visible;
        boxCol.enabled = visible;
    }
}

   
    

