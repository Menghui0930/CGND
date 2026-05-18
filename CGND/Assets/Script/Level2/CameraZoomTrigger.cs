using System.Collections;
using UnityEngine;

/// <summary>
/// 挂在带 Box Collider 2D（Is Trigger = true）的 GameObject 上。
/// 玩家进入后：相机平滑移到指定位置、Size 扩大、停止跟随玩家。
/// 玩家死亡后重置，可以再次触发。
/// </summary>
public class CameraZoomTrigger : MonoBehaviour {
    [Header("Target Position")]
    [SerializeField] private Transform cameraTargetPoint;

    [Header("Camera Size")]
    [SerializeField] private float targetSize = 8f;
    [SerializeField] private float sizeTransitionSpeed = 2f;

    [Header("Move Speed")]
    [SerializeField] private float moveSmoothness = 3f;

    [Header("Blockers")]
    public Collider2D leftBlocker;
    public Collider2D rightBlocker;

    [Header("References")]
    public BossController boss;

    [Header("BGM")]
    [Tooltip("Boss 触发时切换的音乐，不填则保持原本的关卡 BGM")]
    [SerializeField] private AudioClip bossBGM;

    private Camera _cam;
    private bool _triggered = false;

    private void Awake() {
        _cam = Camera.main;
    }

    private void Start() {
        leftBlocker.gameObject.SetActive(false);
        rightBlocker.gameObject.SetActive(false);
    }

    private void OnEnable() => Health.OnDeath += OnPlayerDeath;
    private void OnDisable() => Health.OnDeath -= OnPlayerDeath;

    // 玩家死亡 → 重置触发器，让下次进入可以再次触发
    private void OnPlayerDeath(PlayerMotor playerMotor) {
        StopAllCoroutines();
        _triggered = false;
        AudioManager.Instance.RestoreSceneBGM();

        // 相机立刻恢复跟随（由 BossL2_Health 控制 size，这里只重置 stopFollow）
        Camera2D.instance.SetTargetSmooth(Camera2D.instance.Target);
        Camera2D.instance.stopFollow = false;
        Camera.main.orthographicSize = 6f; // 直接重置，避免玩家复活时 size 还是 8
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;
        leftBlocker.gameObject.SetActive(true);
        rightBlocker.gameObject.SetActive(true);

        // 切换成 Boss BGM
        if (bossBGM != null)
            AudioManager.Instance.PlayBGM(bossBGM);

        StartCoroutine(ZoomAndLock());
    }

    private IEnumerator ZoomAndLock() {
        Camera2D cam2D = Camera2D.instance;
        cam2D.stopFollow = true;

        Vector3 targetPos = new Vector3(
            cameraTargetPoint.position.x,
            cameraTargetPoint.position.y,
            cam2D.transform.localPosition.z
        );

        while (Vector3.Distance(cam2D.transform.localPosition, targetPos) > 0.02f ||
               Mathf.Abs(_cam.orthographicSize - targetSize) > 0.02f) {
            cam2D.transform.localPosition = Vector3.Lerp(
                cam2D.transform.localPosition,
                targetPos,
                moveSmoothness * Time.deltaTime
            );
            _cam.orthographicSize = Mathf.Lerp(
                _cam.orthographicSize,
                targetSize,
                sizeTransitionSpeed * Time.deltaTime
            );
            yield return null;
        }

        cam2D.transform.localPosition = targetPos;
        _cam.orthographicSize = targetSize;

        boss.ActivateBoss();
    }
}