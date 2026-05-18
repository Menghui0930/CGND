using UnityEngine;

public class Boss01Openning : MonoBehaviour
{
    [SerializeField] private BossScript boss;
    [SerializeField] private GameObject bossBoxCol;
    private bool _triggered = false;

    [Header("BGM")]
    [Tooltip("Boss 触发时切换的音乐，不填则保持原本的关卡 BGM")]
    [SerializeField] private AudioClip bossBGM;

    private void Start() {
        bossBoxCol.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (boss == null) return;
        if (collision.CompareTag("Player") && !_triggered) {
            bossBoxCol.gameObject.SetActive(true);
            _triggered = true;

            // 切换成 Boss BGM
            if (bossBGM != null)
                AudioManager.Instance.PlayBGM(bossBGM);

            boss.BossActivate();
            collision.GetComponent<PlayerMotor>().DisableControl();
        }
    }

    private void ResetTrigger(PlayerMotor playerMotor) {
        _triggered = false;
        bossBoxCol.gameObject.SetActive(false);

        AudioManager.Instance.RestoreSceneBGM();
    }

    private void OnEnable() {
        Health.OnDeath += ResetTrigger;
    }

    private void OnDisable() {
        Health.OnDeath -= ResetTrigger;
    }
}
