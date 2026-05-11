using UnityEngine;

public class BossHealth : EnemyHealth {
    private Animator _anim;
    public BoxCollider2D _blockPlayer;
    protected override void Start() {
        base.Start();
        _anim = GetComponent<Animator>();
    }

    public override void TakeDamage(int damage) {

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if(currentHealth <= 0) {
            Die();
            return;   
        }

        _anim.SetTrigger("Hurt");
    }


    protected override void Die() {
        _anim.ResetTrigger("Hurt");
        _anim.SetTrigger("Death");
        _blockPlayer.gameObject.SetActive(false);
        GetComponent<BossScript>().enabled = false;
        GetComponent<BossAudio>().PlayLevelBGM();
        Destroy(gameObject, 2f);
    }
}