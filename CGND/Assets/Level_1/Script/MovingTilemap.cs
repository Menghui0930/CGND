using System.Collections;
using UnityEngine;

public class MovingTilemap : MonoBehaviour {
    public float moveDistance = 3f;
    public float moveSpeed = 2f;
    [SerializeField] private float waitSecond = 1f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isMoving = false;

    void Start() {
        startPos = transform.position;
        targetPos = startPos + Vector3.down * moveDistance;
    }

    void Update() {
        Vector3 destination = isMoving ? targetPos : startPos;
        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            moveSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            other.transform.SetParent(transform);
            StartCoroutine(DelayedMove());   
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            StopAllCoroutines();            
            isMoving = false;
            other.transform.SetParent(null);
        }
    }

    private IEnumerator DelayedMove() {
        yield return new WaitForSeconds(waitSecond);  
        isMoving = true;
    }
}