using Unity.VisualScripting;
using UnityEngine;

public class Camera2D : MonoBehaviour
{
    public static Camera2D instance;

    [Header("Follow Toggle")]
    [SerializeField] private bool horizontalFollow = true;
    [SerializeField] private bool verticalFollow = true;
    public bool stopFollow = false; 

    [Header("Horizontal")]
    [SerializeField][Range(0, 1)] private float horizontalInfluence = 1f;
    [SerializeField] private float horizontalOffset = -3f; 
    [SerializeField] private float horizontalSmoothness = 3f;

    [Header("Vertical")]
    [SerializeField][Range(0, 1)] private float verticalInfluence = 1f;
    [SerializeField] private float verticalOffset = 0f;
    [SerializeField] private float verticalSmoothness = 3f;

    [Header("Camera Bounds")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 100f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 100f;

    private float _targetHorizontalSmoothFollow;
    private float _targetVerticalSmoothFollow;

    public PlayerMotor Target { get; set; }

    private void Awake() {
        instance = this;
    }

    private void Update() {
        MoveCamera();
    }

    private void MoveCamera() {
        if (Target == null) return;

        if (stopFollow) return;

        Vector3 targetPos = GetTargetPosition(Target);

        _targetHorizontalSmoothFollow = Mathf.Lerp(
            _targetHorizontalSmoothFollow, targetPos.x,
            horizontalSmoothness * Time.deltaTime);

        _targetVerticalSmoothFollow = Mathf.Lerp(
            _targetVerticalSmoothFollow, targetPos.y,
            verticalSmoothness * Time.deltaTime);

        float xPos = horizontalFollow ? _targetHorizontalSmoothFollow : transform.localPosition.x;
        float yPos = verticalFollow ? _targetVerticalSmoothFollow : transform.localPosition.y;

        if (useBounds) {
            xPos = Mathf.Clamp(xPos, minX, maxX);
            yPos = Mathf.Clamp(yPos, minY, maxY);
        }

        transform.localPosition = new Vector3(xPos, yPos, transform.localPosition.z);
    }

    private Vector3 GetTargetPosition(PlayerMotor player) {
        float xPos = (player.transform.position.x + horizontalOffset) * horizontalInfluence;
        float yPos = (player.transform.position.y + verticalOffset) * verticalInfluence;
        return new Vector3(xPos, yPos, transform.position.z);
    }

    private void CenterOnTarget(PlayerMotor player) {
        Target = player;

        Vector3 targetPos = GetTargetPosition(Target);
        _targetHorizontalSmoothFollow = targetPos.x;
        _targetVerticalSmoothFollow = targetPos.y;

        transform.localPosition = targetPos;
    }

    private void OnEnable() {
        LevelManager.OnPlayerSpawn += CenterOnTarget;
    }

    private void OnDisable() {
        LevelManager.OnPlayerSpawn -= CenterOnTarget;
    }
}
