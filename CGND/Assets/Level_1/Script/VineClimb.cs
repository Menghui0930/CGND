using UnityEngine;

public class VineClimb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Hand")) {
            Debug.Log("Chuanda Climb");
            other.GetComponentInParent<PlayerClimbing>()?.StartClimb(this);
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Hand")) {
            Debug.Log("Stop Climb");
            other.GetComponentInParent<PlayerClimbing>()?.StopClimb();
            other.GetComponentInParent<PlayerClimbing>()?.CLoseCLimbing();
        }
    }
}
