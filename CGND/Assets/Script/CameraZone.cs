using UnityEngine;

public class CameraZone : MonoBehaviour {
    [Header("Camera Offset Override")]
    [SerializeField] private float horizontalOffset = -3f;
    [SerializeField] private float verticalOffset = 0f;
    [SerializeField] private float transitionSpeed = 2f;  // 过渡速度
    [SerializeField] private float MinY = 0.85f;  // 过渡速度
    [SerializeField] private bool isStopfollowing = false;  

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Camera2D.instance.SetOffsets(horizontalOffset, verticalOffset, transitionSpeed,MinY, isStopfollowing);
        }
    }
}