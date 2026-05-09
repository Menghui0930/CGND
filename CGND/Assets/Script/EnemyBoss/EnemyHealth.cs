using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    [SerializeField] protected int maxHealth = 100;
    protected int currentHealth;

    protected virtual void Start() {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damage) {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void Die() {
        Destroy(gameObject);
    }
}