using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class BossL2_Health : EnemyHealth
{
    private Animator _anim;
    private BossController _bossController;

    [Header("Blockers")]
    public Collider2D leftBlocker;    // left entrance
    public Collider2D rightBlocker;     // right exit

    [Header("Death")]
    public Transform deathPoint;        // 拖入那个 Point GameObject
    public float deathFlySpeed = 3f;    // 飞过去的速度

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable() => Health.OnDeath += OnPlayerDeath;
    private void OnDisable() => Health.OnDeath -= OnPlayerDeath;

    // 玩家死亡 → 重置 Boss 和 blocker
    private void OnPlayerDeath(PlayerMotor playerMotor) {
        StopAllCoroutines();

        // 重置 Boss 行为 & 位置 & 血量
        _bossController.ResetBoss();
        currentHealth = maxHealth;   // EnemyHealth 里重置 currentHealth

        // 关掉 blocker，玩家复活后可以自由走动
        leftBlocker.gameObject.SetActive(false);
        rightBlocker.gameObject.SetActive(false);
    }


    protected override void Start() {
        base.Start();
        _anim = GetComponent<Animator>();
        _bossController = GetComponent<BossController>();
    }

    public override void TakeDamage(int damage) {
        if (!_bossController.bossActive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0) {
            Die();
            return;
        }

        //_anim.SetTrigger("Hurt");
    }

    protected override void Die() {
        // 停止 Boss FSM，不再飞来飞去 / 发子弹
        _bossController.bossActive = false;

        leftBlocker.gameObject.SetActive(false);
        rightBlocker.gameObject.SetActive(false);
        AudioManager.Instance.RestoreSceneBGM();

        StartCoroutine(FlyThenDie());
    }

    private IEnumerator FlyThenDie() {
        // 飞向 deathPoint
        while (Vector2.Distance(transform.position, deathPoint.position) > 0.1f) {
            transform.position = Vector2.MoveTowards(
            transform.position,
                deathPoint.position,
                deathFlySpeed * Time.deltaTime
            );
            yield return null;
        }

        // 精确对齐
        transform.position = deathPoint.position;

        // 播放死亡动画
        _anim.Play("BossDie");

        // 等一帧让 Animator 切换到 BossDie 状态
        yield return null;
        float clipLength = _anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(clipLength);

        // 动画播完 → 相机平滑恢复跟随玩家 + Size 缩回原来的 6
        Camera2D.instance.SetTargetSmooth(Camera2D.instance.Target);
        Camera2D.instance.stopFollow = false;
        StartCoroutine(RestoreCameraSize(6f, 2f));
    }

    private IEnumerator RestoreCameraSize(float targetSize, float speed) {
        Camera cam = Camera.main;
        while (Mathf.Abs(cam.orthographicSize - targetSize) > 0.02f) {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, speed * Time.deltaTime);
            yield return null;
        }
        cam.orthographicSize = targetSize;
    }
}
