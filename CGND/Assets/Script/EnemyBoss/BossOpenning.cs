using UnityEngine;

public class BossOpenning : MonoBehaviour {
    [SerializeField] private FSM boss;
    [SerializeField] private GameObject wallBlock;
    private bool _triggered = false;

    [Header("BGM")]
    [Tooltip("Boss 触发时切换的音乐，不填则保持原本的关卡 BGM")]
    [SerializeField] private AudioClip bossBGM;


    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !_triggered) {
            _triggered = true;
            wallBlock.gameObject.SetActive(true);

            // 切换成 Boss BGM
            if (bossBGM != null)
                AudioManager.Instance.PlayBGM(bossBGM);

            boss.BossActivate();
            other.GetComponent<PlayerMotor>().DisableControl();
        }
    }

    private void ResetTrigger(PlayerMotor playerMotor) {
        _triggered = false;
        wallBlock.gameObject.SetActive(false);

        // 玩家死亡后切回对应关卡的 BGM
        AudioManager.Instance.RestoreSceneBGM();
    }

    private void OnEnable() {
        Health.OnDeath += ResetTrigger;
    }

    private void OnDisable() {
        Health.OnDeath -= ResetTrigger;
    }
}