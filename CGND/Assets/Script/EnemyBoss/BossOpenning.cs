using UnityEngine;

public class BossOpenning : MonoBehaviour {
    [SerializeField] private FSM boss;
    [SerializeField] private GameObject wallBlock;
    private bool _triggered = false;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !_triggered) {
            _triggered = true;
            wallBlock.gameObject.SetActive(true);
            boss.BossActivate();
            other.GetComponent<PlayerMotor>().DisableControl();
        }
    }

    private void ResetTrigger(PlayerMotor playerMotor) {
        _triggered = false;
        wallBlock.gameObject.SetActive(false);
    }

    private void OnEnable() {
        Health.OnDeath += ResetTrigger;
    }

    private void OnDisable() {
        Health.OnDeath -= ResetTrigger;
    }
}