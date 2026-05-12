using UnityEngine;

public class Boss01Openning : MonoBehaviour
{
    [SerializeField] private BossScript boss;
    [SerializeField] private GameObject bossBoxCol;
    private bool _triggered = false;

    private void Start() {
        bossBoxCol.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (boss == null) return;
        if (collision.CompareTag("Player") && !_triggered) {
            bossBoxCol.gameObject.SetActive(true);
            _triggered = true;
            boss.BossActivate();
            collision.GetComponent<PlayerMotor>().DisableControl();
        }
    }

    private void ResetTrigger(PlayerMotor playerMotor) {
        _triggered = false;
        bossBoxCol.gameObject.SetActive(false);
    }

    private void OnEnable() {
        Health.OnDeath += ResetTrigger;
    }

    private void OnDisable() {
        Health.OnDeath -= ResetTrigger;
    }
}
