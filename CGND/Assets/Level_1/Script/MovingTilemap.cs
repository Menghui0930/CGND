using UnityEngine;

public class MovingTilemap : MonoBehaviour
{
    public float moveDistance = 3f;   
    public float moveSpeed = 2f;      

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isMoving = false;

    void Start() {
        startPos = transform.position;
        targetPos = startPos + Vector3.down * moveDistance;
    }

    void Update() {
        if (isMoving) {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
        } else {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPos,
                moveSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isMoving = true;
            other.gameObject.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isMoving = false;
            other.gameObject.transform.SetParent(null);
        }
    }
}
