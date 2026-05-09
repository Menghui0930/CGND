using System.Collections;
using UnityEngine;

public class BossL3Health : EnemyHealth {
    private FSM _fsm;

    protected override void Start() {
        base.Start();
        _fsm = GetComponent<FSM>();
        // 同步到 FSM 的 parameter
        _fsm.parameter.maxHealth = maxHealth;
        _fsm.parameter.currentHealth = maxHealth;
    }

    public override void TakeDamage(int damage) {
        if (_fsm.parameter.shieldObject != null &&
            _fsm.parameter.shieldObject.isActiveAndEnabled) return;  // 有盾免疫

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        _fsm.parameter.currentHealth = currentHealth;   // 同步给 FSM

        // 受击震动
        Camera2D.instance.StartShake(0.1f, 10f);
        StartCoroutine(StopShakeAfter(0.15f));

        if (currentHealth <= 0)
            Die();
    }

    protected override void Die() {
        Debug.Log("Boss Dead");
        // 播死亡动画、过关等
    }

    private IEnumerator StopShakeAfter(float delay) {
        yield return new WaitForSeconds(delay);
        Camera2D.instance.StopShake();
    }
}